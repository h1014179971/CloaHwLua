local IdleGuide = {
		{
			Id = 1,
			ConditionKey = 1,
			ConditionValue = "-1,0",
			NextGuide = 2,
			GuideStepIds = "101,102,103,104,105,106,107,108,109,110,111,112,113",
			NeedComplete = False,
		},
		{
			Id = 2,
			ConditionKey = 1,
			ConditionValue = "-1,0",
			NextGuide = 3,
			GuideStepIds = "201,202,203,204,206",
			NeedComplete = False,
		},
		{
			Id = 3,
			ConditionKey = 1,
			ConditionValue = "-1,0",
			NextGuide = -1,
			GuideStepIds = "301,302,303,305,306",
			NeedComplete = False,
		},
		{
			Id = 4,
			ConditionKey = 2,
			ConditionValue = "30000,0",
			NextGuide = -1,
			GuideStepIds = "401,402,403,404",
			NeedComplete = False,
		},
}
return IdleGuide