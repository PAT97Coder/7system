INSERT dbo.dt312_Groups(DisplayName,CreateAt,CreateBy) VALUES(N'測試題組','2026-10-20','VNW0000001');
INSERT dbo.dt312_Questions(GroupId,DisplayName,IsmultiAns) VALUES(1,N'既有選擇題',0);
INSERT dbo.dt312_Answers(QuesId,DisplayName,TrueAns) VALUES(1,N'正確選項',1),(1,N'錯誤選項',0);
