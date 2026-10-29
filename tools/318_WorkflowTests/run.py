"""Integration tests against a dedicated LocalDB only. Never reads application credentials."""
from pathlib import Path
import argparse
import subprocess
import tempfile
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
INSTANCE = 'Codex318WorkflowValidation'
DATABASE = 'Codex318WorkflowTests'
SERVER = '(localdb)\\' + INSTANCE
NS = {'s': 'http://schemas.microsoft.com/ado/2009/11/edm/ssdl'}


def checked(args, **kwargs):
    subprocess.run(args, check=True, **kwargs)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--artifacts', type=Path)
    args = parser.parse_args()
    work = (args.artifacts or Path(tempfile.mkdtemp(prefix='318_workflow_tests_'))).resolve()
    work.mkdir(parents=True, exist_ok=True)
    print('Test artifacts:', work, flush=True)
    msbuild = Path(r'C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe')
    with (work/'build.log').open('w', encoding='utf-8') as log:
        checked([str(msbuild), str(ROOT/'KnowledgeSystem/KnowledgeSystem.csproj'), '/t:Build', '/p:Configuration=Debug', '/p:Platform=AnyCPU', '/nologo', '/verbosity:quiet'], stdout=log, stderr=subprocess.STDOUT)
    instances = subprocess.run(['SqlLocalDB.exe', 'info'], capture_output=True, text=True, check=True).stdout.splitlines()
    if INSTANCE not in [name.strip() for name in instances]:
        checked(['SqlLocalDB.exe', 'create', INSTANCE, '15.0', '-s'])
    else:
        checked(['SqlLocalDB.exe', 'start', INSTANCE])
    # These identifiers are constants for an isolated instance, never app settings.
    assert SERVER == r'(localdb)\Codex318WorkflowValidation' and DATABASE == 'Codex318WorkflowTests'
    checked(['sqlcmd', '-S', SERVER, '-E', '-d', 'master', '-b', '-Q',
             f"IF DB_ID(N'{DATABASE}') IS NOT NULL DROP DATABASE [{DATABASE}]; CREATE DATABASE [{DATABASE}];"])
    schema = ET.parse(ROOT/'DataAccessLayer/ModelDocManager.edmx').getroot()
    entities = {e.get('Name'): e for e in schema.findall('.//s:EntityType', NS)}
    tables = ['dm_User', 'dm_JobTitle', 'dm_Group', 'dm_GroupUser', 'dm_Attachment', 'sys_NotesMail', 'sys_Log',
              'dt318_Tasks', 'dt318_TaskUsers', 'dt318_TaskAttach', 'dt318_TaskTeams',
              'dt318_TaskWorkflowSteps', 'dt318_TaskHistory']
    statements = ['SET NOCOUNT ON; SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON;']
    for name in tables:
        entity = entities[name]
        fields = []
        for prop in entity.findall('s:Property', NS):
            typ = prop.get('Type')
            if prop.get('MaxLength'):
                typ += '(' + prop.get('MaxLength') + ')'
            if typ in ('numeric', 'decimal'):
                typ += '(' + prop.get('Precision') + ',' + prop.get('Scale') + ')'
            fields.append('['+prop.get('Name')+'] '+typ
                          + (' IDENTITY(1,1)' if prop.get('StoreGeneratedPattern') == 'Identity' else '')
                          + (' NOT NULL' if prop.get('Nullable') == 'false' else ' NULL'))
        fields.append('PRIMARY KEY (' + ','.join('['+p.get('Name')+']' for p in entity.find('s:Key', NS)) + ')')
        statements.append('CREATE TABLE dbo.['+name+']('+','.join(fields)+');')
    statements.append("CREATE UNIQUE INDEX UX_dt318_Tasks_MaNguon ON dbo.dt318_Tasks(MaNguon) WHERE MaNguon IS NOT NULL;")
    for user in ['TEST7810', 'TEST7820', 'REVIEW7810', 'REVIEW7820', 'TESTPSM', 'TESTBOSS', 'OUTSIDER']:
        statements.append(f"INSERT dbo.dm_User(Id,DisplayName,DateCreate) VALUES('{user}',N'{user[:8]}',GETDATE());")
    statements.append("INSERT dbo.dm_JobTitle(Id,DisplayName) VALUES('FIXTURE',N'工程師');")
    statements.append("UPDATE dbo.dm_User SET JobCode='FIXTURE',IdDepartment='7810';")
    statements.append("INSERT dbo.dm_Group(DisplayName,Prioritize) VALUES(N'二級7810',1),(N'二級7820',2),(N'PSM',3),(N'一級',4);")
    statements.append("INSERT dbo.dm_GroupUser(IdGroup,IdUser) VALUES(1,'REVIEW7810'),(2,'REVIEW7820'),(3,'TESTPSM'),(4,'TESTBOSS');")
    fixture = work/'fixture.sql'
    fixture.write_text('\n'.join(statements), encoding='utf-8-sig')
    checked(['sqlcmd','-S',SERVER,'-E','-d',DATABASE,'-b','-f','65001','-i',str(fixture)])
    binary = ROOT/'KnowledgeSystem/bin/Debug'
    executable = binary/'Module318WorkflowTests.exe'
    compiler = msbuild.parent/'Roslyn/csc.exe'
    references = ['BusinessLayer.dll','DataAccessLayer.dll','EntityFramework.dll',
                  'DevExpress.XtraGrid.v22.1.dll','DevExpress.XtraEditors.v22.1.dll',
                  'DevExpress.XtraLayout.v22.1.dll','DevExpress.XtraBars.v22.1.dll',
                  'DevExpress.Utils.v22.1.dll','DevExpress.Data.v22.1.dll']
    checked([str(compiler), '/nologo', '/langversion:7.3', '/target:exe', '/out:'+str(executable),
             '/reference:'+str(binary/'KnowledgeSystem.exe')]
            + ['/reference:'+str(binary/r) for r in references]
            + ['/reference:System.dll','/reference:System.Core.dll','/reference:System.Data.dll',
               '/reference:System.Configuration.dll',
               '/reference:System.Drawing.dll','/reference:System.Windows.Forms.dll',
               str(Path(__file__).with_name('WorkflowTests.cs'))])
    config = ET.Element('configuration')
    sections = ET.SubElement(config, 'configSections')
    ET.SubElement(sections, 'section', name='entityFramework', type='System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework')
    settings = ET.SubElement(config, 'appSettings')
    common = work/'Common'; common.mkdir(exist_ok=True)
    for key,value in {'Module318:UserTuan':'TEST7810','Module318:UserHung':'TEST7820',
                      'Module318:CommonRoot':str(common),'TestFiles':str(work/'Input')}.items():
        ET.SubElement(settings,'add',key=key,value=value)
    providers = ET.SubElement(ET.SubElement(config,'entityFramework'),'providers')
    ET.SubElement(providers,'provider',invariantName='System.Data.SqlClient',type='System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer')
    connections = ET.SubElement(config,'connectionStrings')
    provider = f'Data Source={SERVER};Initial Catalog={DATABASE};Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=10;'
    entity = 'metadata=res://*/ModelDocManager.csdl|res://*/ModelDocManager.ssdl|res://*/ModelDocManager.msl;provider=System.Data.SqlClient;provider connection string="'+provider+'"'
    ET.SubElement(connections,'add',name='DBDocumentManagementSystemEntities',providerName='System.Data.EntityClient',connectionString=entity)
    config_path = Path(str(executable)+'.config')
    ET.ElementTree(config).write(config_path,encoding='utf-8',xml_declaration=True)
    try:
        checked([str(executable)],cwd=work,timeout=120)
    finally:
        # Only files owned by this runner, never application configuration.
        executable.unlink(missing_ok=True)
        config_path.unlink(missing_ok=True)


if __name__ == '__main__':
    main()
