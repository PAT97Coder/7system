-- HSK exams are now completed on the web. Disable the desktop exam page.
UPDATE dbo.dm_Function
SET Status = 0
WHERE ControlName = 'uc314_HskMyExam';
GO

-- Keep the remaining HSK menu items in a continuous display order.
UPDATE dbo.dm_Function
SET Prioritize = CASE ControlName
    WHEN 'uc314_HskQuestionBank' THEN 1
    WHEN 'uc314_HskExamMgmt' THEN 2
END
WHERE ControlName IN ('uc314_HskQuestionBank', 'uc314_HskExamMgmt');
GO
