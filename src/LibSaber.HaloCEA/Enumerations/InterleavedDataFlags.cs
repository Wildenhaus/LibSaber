namespace LibSaber.HaloCEA.Enumerations
{

  [Flags]
  public enum InterleavedDataFlags : long
  {

    UNUSED_00 = 1 << 00,
    UNUSED_01 = 1 << 01,
    UNUSED_02 = 1 << 02,
    UNUSED_03 = 1 << 03,
    UNUSED_04 = 1 << 04,
    UNUSED_05 = 1 << 05,
    UNUSED_06 = 1 << 06,
    UNUSED_07 = 1 << 07,
    UNUSED_08 = 1 << 08,
    UNUSED_09 = 1 << 09,

    Unk_0A = 1 << 10,
    Unk_0B = 1 << 11,

    _TANG0 = 1 << 12,
    _TANG1 = 1 << 13,
    _TANG2 = 1 << 14,
    _TANG3 = 1 << 15,
    UNUSED_10 = 1 << 16, // _TANG4?

    _COMPRESSED_TANG_0 = 1 << 17,
    _COMPRESSED_TANG_1 = 1 << 18,
    _COMPRESSED_TANG_2 = 1 << 19,
    _COMPRESSED_TANG_3 = 1 << 20,
    UNUSED_15 = 1 << 21, // _COMPRESSED_TANG_4?

    _COL0 = 1 << 22,
    _COL1 = 1 << 23,
    _COL2 = 1 << 24,

    _TEX0 = 1 << 25,
    _TEX1 = 1 << 26,
    _TEX2 = 1 << 27,
    _TEX3 = 1 << 28,
    UNUSED_1D = 1 << 29, // _TEX4?

    _COMPRESSED_TEX_0 = 1 << 30,
    _COMPRESSED_TEX_1 = 1 << 31,
    _COMPRESSED_TEX_2 = 1 << 32,
    _COMPRESSED_TEX_3 = 1 << 33,
    UNUSED_22 = 1 << 34, // _COMPRESSED_TEX_4?

    UNUSED_23 = 1 << 35,
    UNUSED_24 = 1 << 36,
    UNUSED_25 = 1 << 37,
    UNUSED_26 = 1 << 38,
    UNUSED_27 = 1 << 39,
    UNUSED_28 = 1 << 40,
    UNUSED_29 = 1 << 41,
    UNUSED_2A = 1 << 42,
    UNUSED_2B = 1 << 43,
    UNUSED_2C = 1 << 44,

    Unk_2D = 1 << 45,

    UNUSED_2E = 1 << 46,
    UNUSED_2F = 1 << 47,

  }

}
