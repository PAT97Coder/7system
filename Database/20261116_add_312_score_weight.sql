-- Storage only. NULL keeps equal-weight scoring until configured.
IF COL_LENGTH(N'dbo.dt312_Setting', N'ChoiceScorePercent') IS NULL
    ALTER TABLE dbo.dt312_Setting ADD ChoiceScorePercent int NULL;
