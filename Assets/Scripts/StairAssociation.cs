using System;
using UnityEngine;

public class StairAssociation
{
	// Note: this type is marked as 'beforefieldinit'.
	static StairAssociation()
	{
		StairAssociation stairAssociation = new StairAssociation();
		stairAssociation.Faces = new int[]
		{
			1,
			0,
			3,
			2,
			4,
			5
		};
		stairAssociation.Forward = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, 0.5f),
			num = new int[]
			{
				0,
				3,
				2,
				1
			},
			uvX = new float[][]
			{
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				},
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				}
			},
			uvY = new float[][]
			{
				new float[]
				{
					0.5f,
					1f,
					1f,
					0.5f
				},
				new float[]
				{
					0f,
					0.5f,
					0.5f,
					0f
				}
			}
		};
		stairAssociation.Top = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(0f, 0f, -0.5f),
			v2 = new Vector3(-0.5f, 0f, 0f),
			num = new int[]
			{
				2,
				3,
				0,
				1
			},
			uvY = new float[][]
			{
				new float[]
				{
					0f,
					1f,
					1f,
					0f
				},
				new float[]
				{
					0f,
					1f,
					1f,
					0f
				}
			},
			uvX = new float[][]
			{
				new float[]
				{
					0f,
					0f,
					0.5f,
					0.5f
				},
				new float[]
				{
					0.5f,
					0.5f,
					1f,
					1f
				}
			}
		};
		stairAssociation.Right = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(-0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[]
			{
				1,
				0,
				2,
				3
			},
			uvY = new float[][]
			{
				new float[]
				{
					0f,
					0.5f,
					0.5f,
					0f
				},
				new float[]
				{
					0f,
					1f,
					1f,
					0f
				}
			},
			uvX = new float[][]
			{
				new float[]
				{
					0.5f,
					0.5f,
					1f,
					1f
				},
				new float[]
				{
					0f,
					0f,
					0.5f,
					0.5f
				}
			}
		};
		StairAssociation stairAssociation2 = stairAssociation;
		StairAssociation.SideAssociation sideAssociation = new StairAssociation.SideAssociation();
		sideAssociation.v1 = new Vector3(-0.5f, 0f, 0f);
		sideAssociation.v2 = new Vector3(0f, 0f, -0.5f);
		StairAssociation.SideAssociation sideAssociation2 = sideAssociation;
		int[] array = new int[4];
		array[0] = 2;
		array[1] = 3;
		array[2] = 1;
		sideAssociation2.num = array;
		sideAssociation.uvY = new float[][]
		{
			new float[]
			{
				0f,
				0.5f,
				0.5f,
				0f
			},
			new float[]
			{
				0f,
				1f,
				1f,
				0f
			}
		};
		sideAssociation.uvX = new float[][]
		{
			new float[]
			{
				0f,
				0f,
				0.5f,
				0.5f
			},
			new float[]
			{
				0.5f,
				0.5f,
				1f,
				1f
			}
		};
		stairAssociation2.Left = sideAssociation;
		StairAssociation.East = stairAssociation;
		stairAssociation = new StairAssociation();
		stairAssociation.Faces = new int[]
		{
			0,
			1,
			2,
			3,
			4,
			5
		};
		stairAssociation.Forward = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(-0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, 0.5f),
			num = new int[]
			{
				0,
				3,
				2,
				1
			},
			uvX = new float[][]
			{
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				},
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				}
			},
			uvY = new float[][]
			{
				new float[]
				{
					0.5f,
					1f,
					1f,
					0.5f
				},
				new float[]
				{
					0f,
					0.5f,
					0.5f,
					0f
				}
			}
		};
		stairAssociation.Top = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(0f, 0f, -0.5f),
			v2 = new Vector3(0.5f, 0f, 0f),
			num = new int[]
			{
				0,
				1,
				2,
				3
			},
			uvY = new float[][]
			{
				new float[]
				{
					0f,
					1f,
					1f,
					0f
				},
				new float[]
				{
					0f,
					1f,
					1f,
					0f
				}
			},
			uvX = new float[][]
			{
				new float[]
				{
					0.5f,
					0.5f,
					1f,
					1f
				},
				new float[]
				{
					0f,
					0f,
					0.5f,
					0.5f
				}
			}
		};
		stairAssociation.Right = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(0.5f, 0f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[]
			{
				1,
				0,
				2,
				3
			},
			uvY = new float[][]
			{
				new float[]
				{
					0f,
					0.5f,
					0.5f,
					0f
				},
				new float[]
				{
					0f,
					1f,
					1f,
					0f
				}
			},
			uvX = new float[][]
			{
				new float[]
				{
					0.5f,
					0.5f,
					1f,
					1f
				},
				new float[]
				{
					0f,
					0f,
					0.5f,
					0.5f
				}
			}
		};
		StairAssociation stairAssociation3 = stairAssociation;
		sideAssociation = new StairAssociation.SideAssociation();
		sideAssociation.v1 = new Vector3(0.5f, 0f, 0f);
		sideAssociation.v2 = new Vector3(0f, 0f, -0.5f);
		StairAssociation.SideAssociation sideAssociation3 = sideAssociation;
		int[] array2 = new int[4];
		array2[0] = 2;
		array2[1] = 3;
		array2[2] = 1;
		sideAssociation3.num = array2;
		sideAssociation.uvY = new float[][]
		{
			new float[]
			{
				0f,
				0.5f,
				0.5f,
				0f
			},
			new float[]
			{
				0f,
				1f,
				1f,
				0f
			}
		};
		sideAssociation.uvX = new float[][]
		{
			new float[]
			{
				0f,
				0f,
				0.5f,
				0.5f
			},
			new float[]
			{
				0.5f,
				0.5f,
				1f,
				1f
			}
		};
		stairAssociation3.Left = sideAssociation;
		StairAssociation.West = stairAssociation;
		stairAssociation = new StairAssociation();
		stairAssociation.Faces = new int[]
		{
			2,
			3,
			0,
			1,
			4,
			5
		};
		stairAssociation.Forward = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(0f, -0.5f, 0f),
			v2 = new Vector3(0f, 0f, 0.5f),
			num = new int[]
			{
				0,
				3,
				2,
				1
			},
			uvX = new float[][]
			{
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				},
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				}
			},
			uvY = new float[][]
			{
				new float[]
				{
					0.5f,
					1f,
					1f,
					0.5f
				},
				new float[]
				{
					0f,
					0.5f,
					0.5f,
					0f
				}
			}
		};
		stairAssociation.Top = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(0f, 0f, -0.5f),
			v2 = new Vector3(0f, 0.5f, 0f),
			num = new int[]
			{
				0,
				3,
				1,
				2
			},
			uvX = new float[][]
			{
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				},
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				}
			},
			uvY = new float[][]
			{
				new float[]
				{
					0.5f,
					1f,
					1f,
					0.5f
				},
				new float[]
				{
					0f,
					0.5f,
					0.5f,
					0f
				}
			}
		};
		StairAssociation stairAssociation4 = stairAssociation;
		sideAssociation = new StairAssociation.SideAssociation();
		sideAssociation.v1 = new Vector3(0f, 0.5f, 0f);
		sideAssociation.v2 = new Vector3(0f, 0f, -0.5f);
		StairAssociation.SideAssociation sideAssociation4 = sideAssociation;
		int[] array3 = new int[4];
		array3[0] = 2;
		array3[1] = 3;
		array3[2] = 1;
		sideAssociation4.num = array3;
		sideAssociation.uvY = new float[][]
		{
			new float[]
			{
				0f,
				0.5f,
				0.5f,
				0f
			},
			new float[]
			{
				0f,
				1f,
				1f,
				0f
			}
		};
		sideAssociation.uvX = new float[][]
		{
			new float[]
			{
				0f,
				0f,
				0.5f,
				0.5f
			},
			new float[]
			{
				0.5f,
				0.5f,
				1f,
				1f
			}
		};
		stairAssociation4.Right = sideAssociation;
		stairAssociation.Left = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(0f, 0.5f, 0f),
			v2 = new Vector3(0f, 0f, -0.5f),
			num = new int[]
			{
				1,
				0,
				2,
				3
			},
			uvY = new float[][]
			{
				new float[]
				{
					0f,
					0.5f,
					0.5f,
					0f
				},
				new float[]
				{
					0f,
					1f,
					1f,
					0f
				}
			},
			uvX = new float[][]
			{
				new float[]
				{
					0.5f,
					0.5f,
					1f,
					1f
				},
				new float[]
				{
					0f,
					0f,
					0.5f,
					0.5f
				}
			}
		};
		StairAssociation.North = stairAssociation;
		stairAssociation = new StairAssociation();
		stairAssociation.Faces = new int[]
		{
			3,
			2,
			1,
			0,
			4,
			5
		};
		stairAssociation.Forward = new StairAssociation.SideAssociation
		{
			v1 = new Vector3(0f, 0.5f, 0f),
			v2 = new Vector3(0f, 0f, 0.5f),
			num = new int[]
			{
				0,
				3,
				2,
				1
			},
			uvX = new float[][]
			{
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				},
				new float[]
				{
					0f,
					0f,
					1f,
					1f
				}
			},
			uvY = new float[][]
			{
				new float[]
				{
					0.5f,
					1f,
					1f,
					0.5f
				},
				new float[]
				{
					0f,
					0.5f,
					0.5f,
					0f
				}
			}
		};
		StairAssociation stairAssociation5 = stairAssociation;
		sideAssociation = new StairAssociation.SideAssociation();
		sideAssociation.v1 = new Vector3(0f, 0f, -0.5f);
		sideAssociation.v2 = new Vector3(0f, -0.5f, 0f);
		sideAssociation.num = new int[]
		{
			1,
			2,
			0,
			3
		};
		sideAssociation.uvX = new float[][]
		{
			new float[]
			{
				0f,
				0f,
				1f,
				1f
			},
			new float[]
			{
				0f,
				0f,
				1f,
				1f
			}
		};
		StairAssociation.SideAssociation sideAssociation5 = sideAssociation;
		float[][] array4 = new float[2][];
		int num = 0;
		float[] array5 = new float[4];
		array5[1] = 0.5f;
		array5[2] = 0.5f;
		array4[num] = array5;
		array4[1] = new float[]
		{
			0.5f,
			1f,
			1f,
			0.5f
		};
		sideAssociation5.uvY = array4;
		stairAssociation5.Top = sideAssociation;
		StairAssociation stairAssociation6 = stairAssociation;
		sideAssociation = new StairAssociation.SideAssociation();
		sideAssociation.v1 = new Vector3(0f, -0.5f, 0f);
		sideAssociation.v2 = new Vector3(0f, 0f, -0.5f);
		StairAssociation.SideAssociation sideAssociation6 = sideAssociation;
		int[] array6 = new int[4];
		array6[0] = 2;
		array6[1] = 3;
		array6[2] = 1;
		sideAssociation6.num = array6;
		StairAssociation.SideAssociation sideAssociation7 = sideAssociation;
		float[][] array7 = new float[2][];
		int num2 = 0;
		float[] array8 = new float[4];
		array8[1] = 0.5f;
		array8[2] = 0.5f;
		array7[num2] = array8;
		int num3 = 1;
		float[] array9 = new float[4];
		array9[1] = 1f;
		array9[2] = 1f;
		array7[num3] = array9;
		sideAssociation7.uvY = array7;
		sideAssociation.uvX = new float[][]
		{
			new float[]
			{
				0f,
				0f,
				0.5f,
				0.5f
			},
			new float[]
			{
				0.5f,
				0.5f,
				1f,
				1f
			}
		};
		stairAssociation6.Right = sideAssociation;
		StairAssociation stairAssociation7 = stairAssociation;
		sideAssociation = new StairAssociation.SideAssociation();
		sideAssociation.v1 = new Vector3(0f, -0.5f, 0f);
		sideAssociation.v2 = new Vector3(0f, 0f, -0.5f);
		sideAssociation.num = new int[]
		{
			1,
			0,
			2,
			3
		};
		StairAssociation.SideAssociation sideAssociation8 = sideAssociation;
		float[][] array10 = new float[2][];
		int num4 = 0;
		float[] array11 = new float[4];
		array11[1] = 0.5f;
		array11[2] = 0.5f;
		array10[num4] = array11;
		int num5 = 1;
		float[] array12 = new float[4];
		array12[1] = 1f;
		array12[2] = 1f;
		array10[num5] = array12;
		sideAssociation8.uvY = array10;
		sideAssociation.uvX = new float[][]
		{
			new float[]
			{
				0.5f,
				0.5f,
				1f,
				1f
			},
			new float[]
			{
				0f,
				0f,
				0.5f,
				0.5f
			}
		};
		stairAssociation7.Left = sideAssociation;
		StairAssociation.South = stairAssociation;
	}

	public static StairAssociation GetAssociation(BlockKind kind)
	{
		switch (kind)
		{
		case BlockKind.StairNorth:
			return StairAssociation.North;
		case BlockKind.StairSouth:
			return StairAssociation.South;
		case BlockKind.StairWest:
			return StairAssociation.West;
		case BlockKind.StairEast:
			return StairAssociation.East;
		default:
			return null;
		}
	}

	public BlockFaceStair this[int i]
	{
		get
		{
			return (BlockFaceStair)this.Faces[i];
		}
	}

	public int[] Faces;

	public StairAssociation.SideAssociation Forward;

	public StairAssociation.SideAssociation Top;

	public StairAssociation.SideAssociation Right;

	public StairAssociation.SideAssociation Left;

	public static StairAssociation East;

	public static StairAssociation West;

	public static StairAssociation North;

	public static StairAssociation South;

	public class SideAssociation
	{
		public Vector3 v1 = Vector3.zero;

		public Vector3 v2 = Vector3.zero;

		public int[] num;

		public float[][] uvY;

		public float[][] uvX;
	}
}
