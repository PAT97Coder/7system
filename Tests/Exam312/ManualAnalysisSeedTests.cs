using BusinessLayer;
using DataAccessLayer;
using KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using H = Exam312Tests;

internal static class ManualAnalysisSeedTests
{
    private const string Tag = "[T312A_20261104]", Actor = "VNW9999990", Alpha = "VNW9900311";
    private const int Role = 32912, Function = 32912;

    private static void ExecuteFile(string fileName)
    {
        var entity = new EntityConnectionStringBuilder(SingleConnection.ConString);
        var provider = new SqlConnectionStringBuilder(entity.ProviderConnectionString);
        H.Check(Regex.IsMatch(provider.InitialCatalog,"^7system_Exam312_Test_[0-9a-f]{32}$"),"Manual seed cannot run outside disposable database");
        string directory = Path.GetFullPath(Path.Combine(Environment.GetEnvironmentVariable("EXAM312_TEST_WORKSPACE"),"Database"));
        string script = File.ReadAllText(Path.Combine(directory,fileName));
        const string use = "USE [DBDocumentManagementSystem];";
        H.Check(script.Contains(use),"Manual script target guard missing");
        script=script.Replace(use,"USE ["+provider.InitialCatalog+"];");
        H.Check(!script.Contains("DBDocumentManagementSystem"),"Manual script still targets application database");
        using(var connection=new SqlConnection(provider.ConnectionString))
        {
            connection.Open();
            foreach(string batch in Regex.Split(script,@"^\s*GO\s*$",RegexOptions.Multiline|RegexOptions.IgnoreCase))
                if(!string.IsNullOrWhiteSpace(batch))
                    using(var command=new SqlCommand(batch,connection)) { command.CommandTimeout=30; command.ExecuteNonQuery(); }
        }
    }

    private static string Signature()
    {
        using(var db=new DBDocumentManagementSystemEntities())
            return JsonConvert.SerializeObject(new {
                People=db.dm_User.OrderBy(x=>x.Id).ToList(),Groups=db.dt312_Groups.OrderBy(x=>x.Id).ToList(),
                Questions=db.dt312_Questions.OrderBy(x=>x.Id).ToList(),Answers=db.dt312_Answers.OrderBy(x=>x.Id).ToList(),
                Exams=db.dt312_ExamMgmt.OrderBy(x=>x.Id).ToList(),Papers=db.dt312_ExamUser.OrderBy(x=>x.Id).ToList(),
                Settings=db.dt312_Setting.OrderBy(x=>x.id).ToList(),Grants=db.dm_FunctionRole.OrderBy(x=>x.Id).ToList()
            });
    }

    internal static void Run()
    {
        using(var db=new DBDocumentManagementSystemEntities())
        {
            db.dm_Function.Add(new dm_Function { Id=Function,IdParent=0,ControlName="uc312_ExamAnalysis",DisplayName="測試驗證" });
            db.dm_FunctionRole.Add(new dm_FunctionRole { IdFunction=Function,IdRole=Role });
            db.dm_UserRole.Add(new dm_UserRole { IdUser=Actor,IdRole=Role }); db.SaveChanges();
        }
        string original=Signature();
        ExecuteFile("20261104_seed_312_analysis_demo.sql");
        var bus=dt312_ExamAnalysisBUS.Instance;
        int examA,examB;
        Dictionary<string,int> groups;
        using(var db=new DBDocumentManagementSystemEntities())
        {
            examA=db.dt312_ExamMgmt.Single(x=>x.DisplayName==Tag+" 2024期").Id;
            examB=db.dt312_ExamMgmt.Single(x=>x.DisplayName==Tag+" 2025期").Id;
            var ownedGroups=db.dt312_Groups.Where(x=>x.DisplayName.StartsWith(Tag)).ToList();
            groups=ownedGroups.ToDictionary(x=>x.DisplayName.Substring(Tag.Length+1),x=>x.Id);
            H.Check(ownedGroups.Count==3 && ownedGroups.All(x=>x.RemoveAt.HasValue),"Demo groups can enter live random exam bank");
            H.Check(db.dt312_Questions.Count(x=>groups.Values.Contains(x.GroupId))==12,"Demo question count wrong");
            var exams=db.dt312_ExamMgmt.Where(x=>x.DisplayName.StartsWith(Tag)).Select(x=>x.Id).ToList();
            var papers=db.dt312_ExamUser.Where(x=>exams.Contains(x.ExamId)).ToList();
            H.Check(papers.Count==9 && papers.All(x=>x.SubmitAt.HasValue),"Demo paper count wrong");
            foreach(var paper in papers)
            {
                var rows=JsonConvert.DeserializeObject<List<f312_DoExam.ExamResult>>(paper.ExamData);
                H.Check(rows.Count(r=>r.Questions.IsEssay)==3,"Demo essays missing");
                var mc=rows.Where(r=>!r.Questions.IsEssay).ToList();
                H.Check(mc.All(r=>r.IsCorrect==dt312_ExamScoring.IsCorrect(r.CorrectAnswer,r.UserAnswer)),"Demo JSON grading inconsistent");
                H.Check(mc.All(r=>r.Answers.Select(a=>a.Id).SequenceEqual(new[]{1,2,3,4})),"Demo snapshot must use renumbered answer IDs");
                H.Check(paper.Score==dt312_ExamScoring.Score(mc.Count(r=>r.IsCorrect),mc.Count),"Demo score differs from actual scoring");
                H.Check(f312_ExamReview.BuildHtml(rows,paper.Score,false).Contains(Tag),"Demo paper cannot be opened in review");
            }
        }
        Check(bus.GetStatistics(Actor,examA,Alpha),groups,new[]{2,1,0},2,1,1);
        Check(bus.GetStatistics(Actor,examB,null),groups,new[]{6,4,3},9,3,3);
        Check(bus.GetStatistics(Actor,null,Alpha),groups,new[]{3,5,3},6,1,3);
        Check(bus.GetStatistics(Actor,null,null),groups,new[]{10,9,8},18,3,9);
        string created=Signature();
        try { ExecuteFile("20261104_seed_312_analysis_demo.sql"); throw new Exception("Repeated manual seed overwrites existing data"); }
        catch(SqlException ex) when(ex.Number==51000) { H.Check(created==Signature(),"Repeated seed changed existing data"); }
        // Cleanup must stop before deleting anything when a test profile was granted an unrelated role.
        using(var db=new DBDocumentManagementSystemEntities())
        { db.dm_UserRole.Add(new dm_UserRole { IdUser=Alpha,IdRole=Role }); db.SaveChanges(); }
        try { ExecuteFile("20261104_cleanup_312_analysis_demo.sql"); throw new Exception("Cleanup ignored unexpected test profile usage"); }
        catch(SqlException ex) when(ex.Number==51000)
        {
            using(var db=new DBDocumentManagementSystemEntities())
            { H.Check(db.dt312_ExamMgmt.Any(x=>x.Id==examA),"Rejected cleanup partially deleted exams"); db.dm_UserRole.RemoveRange(db.dm_UserRole.Where(x=>x.IdUser==Alpha)); db.SaveChanges(); }
        }
        ExecuteFile("20261104_cleanup_312_analysis_demo.sql");
        H.Check(original==Signature(),"Cleanup did not exactly restore original rows/configuration/permissions");
        ExecuteFile("20261104_cleanup_312_analysis_demo.sql");
        H.Check(original==Signature(),"Repeated cleanup affects existing data");
        Console.WriteLine("PASS: manual SQL seed/cleanup: full review snapshots, exact 4-scope expected counts, repeat-seed rejection, scoped cleanup guard, complete removal and original data preserved.");
    }

    private static void Check(Exam312AnalysisResult result,Dictionary<string,int> groups,int[] wrong,int total,int people,int papers)
    {
        int i=0;
        foreach(string name in new[]{"安全","設備","流程"})
        {
            var row=result.Rows.Single(r=>r.GroupId==groups[name]);
            H.Check(row.QuestionCount==total && row.WrongCount==wrong[i] && row.PeopleCount==people && row.PaperCount==papers
                && row.ErrorRate==wrong[i]*100m/total,"Manual demo expected counts differ: "+name);
            i++;
        }
    }
}
