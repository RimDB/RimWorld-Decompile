﻿using System;

namespace Verse
{
	// Token: 0x02000BF7 RID: 3063
	public class LanguageWorker_Russian : LanguageWorker
	{
		// Token: 0x060042E1 RID: 17121 RVA: 0x002363B8 File Offset: 0x002347B8
		public override string OrdinalNumber(int number)
		{
			return number + "-й";
		}

		// Token: 0x060042E2 RID: 17122 RVA: 0x002363E0 File Offset: 0x002347E0
		public override string PostProcessed(string str)
		{
			str = base.PostProcessed(str);
			string a = str.Substring(2);
			if ((str.EndsWith("1 ", StringComparison.OrdinalIgnoreCase) || str.EndsWith("2 ", StringComparison.OrdinalIgnoreCase) || str.EndsWith("3 ", StringComparison.OrdinalIgnoreCase) || str.EndsWith("4 ", StringComparison.OrdinalIgnoreCase)) && (a == "лет" || a == "сезонов" || a == "дней" || a == "часов"))
			{
				str = str.Insert(1, "n");
			}
			str = str.Replace("1 лет", "1 год");
			str = str.Replace("1 сезонов", "1 сезон");
			str = str.Replace("1 дней", "1 день");
			str = str.Replace("1 часов", "1 час");
			str = str.Replace("2 лет", "2 года");
			str = str.Replace("2 сезонов", "2 сезона");
			str = str.Replace("2 дней", "2 дня");
			str = str.Replace("2 часов", "2 часа");
			str = str.Replace("3 лет", "3 года");
			str = str.Replace("3 сезонов", "3 сезона");
			str = str.Replace("3 дней", "3 дня");
			str = str.Replace("3 часов", "3 часа");
			str = str.Replace("4 лет", "4 года");
			str = str.Replace("4 сезонов", "4 сезона");
			str = str.Replace("4 дней", "4 дня");
			str = str.Replace("4 часов", "4 часа");
			return str;
		}
	}
}
