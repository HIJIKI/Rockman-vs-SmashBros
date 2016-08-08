using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockman_vs_SmashBros
{
	public static class IntExtensions
	{
		/// <summary>
		/// 指定した整数を、下から n 桁分取得する
		/// </summary>
		/// <param name="Number">取得元となる整数</param>
		/// <param name="Digit">取得したい桁数</param>
		public static int ToLastDigits(this int Number, int Digit)
		{
			int Result = Number;
			if (Number.ToDigit() > Digit)
			{
				string NumberString = Number.ToString();
				Result = int.Parse(NumberString.Substring(NumberString.Length - Digit));
			}
			return Result;
		}

		/// <summary>
		/// 指定した整数の桁数を取得する
		/// </summary>
		/// <param name="Number">桁数を取得したい整数</param>
		public static int ToDigit(this int Number)
		{
			string NumberString = Number.ToString();
			int Result = NumberString.Length;
			return Result;
		}
	}
}
