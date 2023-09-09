using System;

namespace UnityEngine.Purchasing.Security
{
	public class AppleTangle
	{
		public static byte[] Data()
		{
			if (!AppleTangle.IsPopulated)
			{
				return null;
			}
			return Obfuscator.DeObfuscate(AppleTangle.data, AppleTangle.order, AppleTangle.key);
		}

		private static byte[] data = Convert.FromBase64String("oK6n4rG2o6ymo7Cm4ransK+x4qN12X9RgObQ6AXN33SPXpyhColC1cqc8kDD08TBl9/ixkDDyvJAw8byxsTRwJeR89Hy08TBl8bI0ciDsrJ8NrFZLBCmzQm7jfYaYPw7uj2pCgvbsDefzBe9nVkw58F4l02Pn88z4q2k4raqp+K2qqes4qOysq6roaMb9L0DRZcbZVt78IA5GhezXLxjkP/kpeJI8ag1z0ANHClh7TuRqJmm4qOspuKhp7C2q6SroaO2q62s4rKHvN2OqZJUg0sGtqDJ0kGDRfFIQ6ym4qGtrKartqutrLHiraTit7GnTbFDogTZmcvtUHA6hooyovpc1zeQp66ro6yhp+KtrOK2qqux4qGnsPfw8/by8fSY1c/x9/Lw8vvw8/byaWGzUIWRlwNt7YNxOjkhsg8kYY5z8poumMbwTqpxTd8cp7E9pZynfqukq6GjtqutrOKDt7aqrbCrtrvzV1y4zmaFSZkW1PXxCQbNjwzWqxOyrqfigaewtqukq6GjtqutrOKDt/H0mPKg88nyy8TBl8bE0cCXkfPRyunEw8fHxcDD1Nyqtraysfjt7bXmICkTdbIdzYcj5Qgzr7ovJXfV1UDDwsTL6ESKRDWhpsfD8kMw8ujEu+KjsbG3r6ex4qOhoaeytqOsoafHwsFAw83C8kDDyMBAw8PCJlNry72Dalo7EwikXuap0xJheSbZ6AHd6ESKRDXPw8PHx8LyoPPJ8svEwZeLGrRd8danY7VWC+/AwcPCw2FAw8/Ey+hEikQ1z8PDx8fCwUDDw8Ke3VMZ3IWSKccvnLtG7yn0YJWOly6bZcfLvtWClNPcthF1SeH5hWEXrab34deJ15vfcVY1NF5cDZJ4A5qSSdtLHDuJrjfFaeDywCra/DqSyxF3+G82zczCUMlz49Tsthf+zxmg1O7ioaewtqukq6Gjtqfisq2uq6G78tPEwZfGyNHIg7Kyrqfii6yh7PPigYPyQMPg8s/Ey+hEikQ1z8PDw0LW6RKrhVa0yzw2qU/sgmQ1hY+98kDGefJAwWFiwcDDwMDDwPLPxMvEwZffzMbUxtbpEquFVrTLPDapT+3yQwHEyunEw8fHxcDA8kN02ENxtqqtsKu2u/PU8tbEwZfGwdHPg7Kwo6G2q6Gn4rG2o7anr6estrHs8tTy1sTBl8bB0c+DsrKup+KQra223UdBR9lb/4X1MGtZgkzuFnNS0BrE8s3EwZff0cPDPcbH8sHDwz3y381f/zHpi+rYCjwMd3vMG5zeFAn/tqukq6GjtqfioLvio6y74rKjsLayrqfikK2ttuKBg/Lc1c/y9PL28K6n4ousoezz5PLmxMGXxsnR34OypU3KduI1CW7u4q2ydP3D8k51gQ3FLr/7QUmR4hH6BnN9WI3IqT3pPmoevOD3COcXG80UqRZg5uHTNWNutbXso7Kyrqfsoa2v7aOysq6noaO48kDDtPLMxMGX383Dwz3GxsHAw/Rbju+6dS9OWR4xtVkwtBC18o0DAqHxtTX4xe6UKRjN48wYeLHbjXfsgmQ1hY+9ypzy3cTBl9/hxtry1OTy5sTBl8bJ0d+DsrKup+KBp7C2kmhIFxgmPhLLxfVyt7fj");

		private static int[] order = new int[]
		{
			42,
			59,
			33,
			33,
			50,
			19,
			41,
			58,
			56,
			45,
			25,
			43,
			55,
			38,
			58,
			24,
			26,
			43,
			18,
			50,
			56,
			35,
			53,
			34,
			40,
			58,
			43,
			59,
			59,
			47,
			40,
			52,
			56,
			43,
			34,
			44,
			45,
			38,
			56,
			42,
			40,
			56,
			50,
			46,
			55,
			49,
			47,
			53,
			50,
			56,
			53,
			55,
			55,
			54,
			56,
			59,
			56,
			57,
			59,
			59,
			60
		};

		private static int key = 194;

		public static readonly bool IsPopulated = true;
	}
}
