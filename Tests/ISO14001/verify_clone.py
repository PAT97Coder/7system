"""Check copy fidelity, EF metadata and project wiring without a database."""
from pathlib import Path
import re
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]


def convert(text):
    for old, new in [
        ("_01_ISOAuditDocs", "_09_ISO14001Docs"),
        ("01_ISOAuditDocs", "09_ISO14001Docs"),
        ("dt201", "dt209"), ("f201", "f209"), ("uc201", "uc209"),
        ("Iso201", "Iso209"), ("Folder201", "Folder209"),
        ("DocTypes201", "DocTypes209"), ('"201"', '"209"'),
        ("201DocTypes", "209DocTypes"), ("[201]", "[209]"),
        ("ISO組", "ISO14001組"), ("ISO文件", "ISO14001文件"),
        ("ISO 17025", "ISO14001"),
        ("TPConfigs.FolderCalibCert", "TPConfigs.Folder209CalibCert"),
    ]:
        text = text.replace(old, new)
    return text


ui = ROOT / "KnowledgeSystem/Views/02_StandardsAndTechs/01_ISOAuditDocs"
sources = list(ui.glob("*")) + list((ROOT / "BusinessLayer/dt201").glob("*.cs"))
sources += list((ROOT / "DataAccessLayer").glob("dt201_*.cs"))
sources += [ROOT / "KnowledgeSystem/Helpers/Iso201Permission.cs",
            ROOT / "KnowledgeSystem/AppResources/f201_NotifyCalibCert.html"]
removed = ("UpdateUsrReq", "ReqUpdateDocs")
sources = [p for p in sources if not any(name in p.name for name in removed)]
assert len(sources) == 50
for source in sources:
    target = ROOT / convert(source.relative_to(ROOT).as_posix())
    if source.name not in ("uc201_AuditISOMain.cs", "uc201_AuditISOMain.Designer.cs"):
        assert target.read_text(encoding="utf-8-sig") == convert(source.read_text(encoding="utf-8-sig")), target
    if target.suffix == ".cs":
        assert not re.search(r"\b(?:SqlCommand|SqlQuery|ExecuteSqlCommand|ExecuteSqlRaw|SqlConnection)\b", target.read_text(encoding="utf-8-sig")), target
print("PASS: retained files match 201 except the customized 209 document list; no runtime SQL added.")

designer = (ROOT / "KnowledgeSystem/Views/02_StandardsAndTechs/09_ISO14001Docs/uc209_AuditISOMain.Designer.cs").read_text(encoding="utf-8-sig")
columns = [
    ("treeListColumn4", "條文"), ("treeListColumn1", "文件編號"),
    ("treeListColumn5", "文件類"), ("tlsColDept", "部門代號"),
    ("treeListColumn2", "中文名稱"), ("treeListColumn6", "越文名稱"),
    ("treeListColumn10", "附屬表單編碼"), ("treeListColumn11", "附屬表單中文"),
    ("colRecordNameVN", "附屬表單越文"), ("treeListColumn3", "備註"),
]
for index, (name, caption) in enumerate(columns):
    assert f'this.{name}.Caption = "{caption}";' in designer, caption
    assert f'this.{name}.VisibleIndex = {index};' in designer, caption
print("PASS: 209 document columns match the requested captions and order.")

root = ET.parse(ROOT / "DataAccessLayer/ModelDocManager.edmx").getroot()
for namespace in ["http://schemas.microsoft.com/ado/2009/11/edm/ssdl", "http://schemas.microsoft.com/ado/2009/11/edm"]:
    tag = "{" + namespace + "}"
    entities = {e.get("Name"): e for e in root.iter(tag + "EntityType")}
    source_names = [name for name in entities if name.startswith("dt201_") and not any(part in name for part in removed)]
    assert len(source_names) == 6
    for name in source_names:
        original = ET.tostring(entities[name], encoding="unicode").strip()
        copied = ET.tostring(entities[convert(name)], encoding="unicode").strip()
        assert convert(original) == copied, name
print("PASS: all six EF6 entities have matching storage and conceptual definitions.")

msbuild = "{http://schemas.microsoft.com/developer/msbuild/2003}"
for project in ["DataAccessLayer/DataAccessLayer.csproj", "BusinessLayer/BusinessLayer.csproj", "KnowledgeSystem/KnowledgeSystem.csproj"]:
    path = ROOT / project
    entries = list(ET.parse(path).getroot().iter())
    for entry in entries:
        include = entry.get("Include", "")
        if any(key in include for key in ["dt201", "f201", "uc201", "Iso201"]) and not any(part in include for part in removed):
            matches = [e for e in entries if e.tag == entry.tag and e.get("Include") == convert(include)]
            assert len(matches) == 1, (project, include)
            dep = entry.find(msbuild + "DependentUpon")
            if dep is not None:
                assert matches[0].find(msbuild + "DependentUpon").text == convert(dep.text)
print("PASS: all project entries, Designer dependencies and resources registered exactly once.")

personnel = (ROOT / "KnowledgeSystem/Views/04_SystemAdministrator/01_Moderator/f401_UserInfo.cs").read_text(encoding="utf-8-sig")
source_events = re.findall(r"dt201_UpdateUsrReqBUS.Instance.Add\(updateReq\);", personnel)
copied_events = re.findall(r"dt201_UpdateUsrReqBUS.Instance.Add\(updateReq\);\s*dt209_UpdateUsrReqBUS.Instance.Add\(new dt209_UpdateUsrReq\s*\{\s*DateCreate = updateReq.DateCreate, IdDept = updateReq.IdDept,\s*IdUsr = updateReq.IdUsr, TypeChange = updateReq.TypeChange,\s*Describe = updateReq.Describe\s*\}\);", personnel)
assert len(source_events) == 5 and len(copied_events) == 0
assert "dt209_UpdateUsrReqBUS" not in personnel
main = (ROOT / "KnowledgeSystem/Views/00_Generals/f00_Main.Designer.cs").read_text(encoding="utf-8-sig")
assert "btnISO14001Docs" not in main
print("PASS: separate ISO14001 tile and automatic 209 personnel events removed; 201 events retained.")

for folder in ["BusinessLayer/dt209", "KnowledgeSystem/Views/02_StandardsAndTechs/09_ISO14001Docs"]:
    for path in (ROOT / folder).glob("*.cs"):
        assert not any(name in path.read_text(encoding="utf-8-sig") for name in ["dt209_UpdateUsrReq", "dt209_ReqUpdateDocs", "uc209_UpdateUsrReq", "f209_UpdateUsrReq"]), path
print("PASS: removed personnel feature has no remaining runtime references.")
