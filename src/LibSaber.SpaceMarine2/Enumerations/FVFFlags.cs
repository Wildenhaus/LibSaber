namespace LibSaber.SpaceMarine2.Enumerations;

[Flags]
public enum FVFFlags : ulong
{
  #pragma warning disable format
  // @formatter:off — disable formatter after this line
  
  NONE                              = 0,

  VERT                              = 1ul << 0x00, // 0x1
  VERT_4D                           = 1ul << 0x01, // 0x2
  VERT_2D                           = 1ul << 0x02, // 0x4
  VERT_COMPR                        = 1ul << 0x03, // 0x8
  MASKING_FLAGS                     = 1ul << 0x04, // 0x10
  BS_INFO                           = 1ul << 0x05, // 0x20

  WEIGHT4                           = 1ul << 0x06, // 0x40  
  WEIGHT8                           = 1ul << 0x07, // 0x80  
  INDICES                           = 1ul << 0x08, // 0x100 
  INDICES16                         = 1ul << 0x09, // 0x200 
  NORM                              = 1ul << 0x0A, // 0x400 
  NORM_COMPR                        = 1ul << 0x0B, // 0x800 
  NORM_IN_VERT4                     = 1ul << 0x0C, // 0x1000

  TANG0                             = 1ul << 0x0D, // 0x2000
  TANG1                             = 1ul << 0x0E, // 0x4000
  TANG2                             = 1ul << 0x0F, // 0x8000
  TANG3                             = 1ul << 0x10, // 0x10000
  TANG4                             = 1ul << 0x11, // 0x20000
  TANG_COMPR                        = 1ul << 0x12, // 0x40000

  COLOR0                            = 1ul << 0x13, // 0x80000
  COLOR1                            = 1ul << 0x14, // 0x100000
  COLOR2                            = 1ul << 0x15, // 0x200000
  COLOR3                            = 1ul << 0x16, // 0x400000
  COLOR4                            = 1ul << 0x17, // 0x800000
  COLOR5                            = 1ul << 0x18, // 0x1000000

  TEX0                              = 1ul << 0x19, // 0x2000000
  TEX1                              = 1ul << 0x1A, // 0x4000000
  TEX2                              = 1ul << 0x1B, // 0x8000000
  TEX3                              = 1ul << 0x1C, // 0x10000000
  TEX4                              = 1ul << 0x1D, // 0x20000000
  TEX5                              = 1ul << 0x1E, // 0x40000000
  TEX0_COMPR                        = 1ul << 0x1F, // 0x80000000
  TEX1_COMPR                        = 1ul << 0x20, // 0x100000000
  TEX2_COMPR                        = 1ul << 0x21, // 0x200000000
  TEX3_COMPR                        = 1ul << 0x22, // 0x400000000
  TEX4_COMPR                        = 1ul << 0x23, // 0x800000000
  TEX5_COMPR                        = 1ul << 0x24, // 0x1000000000
  TEX0_4D                           = 1ul << 0x25, // 0x2000000000
  TEX1_4D                           = 1ul << 0x26, // 0x4000000000
  TEX2_4D                           = 1ul << 0x27, // 0x8000000000
  TEX3_4D                           = 1ul << 0x28, // 0x10000000000
  TEX4_4D                           = 1ul << 0x29, // 0x20000000000
  TEX5_4D                           = 1ul << 0x2A, // 0x40000000000
  TEX0_4D_BYTE                      = 1ul << 0x2B, // 0x80000000000
  TEX1_4D_BYTE                      = 1ul << 0x2C, // 0x100000000000
  TEX2_4D_BYTE                      = 1ul << 0x2D, // 0x200000000000
  TEX3_4D_BYTE                      = 1ul << 0x2E, // 0x400000000000
  TEX4_4D_BYTE                      = 1ul << 0x2F, // 0x800000000000
  TEX5_4D_BYTE                      = 1ul << 0x30, // 0x1000000000000

  #pragma warning restore format
}