﻿namespace LibSaber.SpaceMarine2.Enumerations;

[Flags]
public enum ObjectStateFlags
{
  VERT_WCS                       = 0x01,
  VALIDATE_GROUP                 = 0x02,
  SKIN_REGULAR                   = 0x04,
  SKIN_COMPOUND                  = 0x08,
  SKIN_WEIGHT_BLENDED            = 0x10,
  NO_FOG                         = 0x20,
  NO_SPOT                        = 0x40,
  DOUBLE_SIDED                   = 0x80,
  START_OFF_MATRMODEL            = 0x100,
  IDENTITY_MODEL_MATR            = 0x200,
  BELONG_TO_STAT_SCENE           = 0x400,
  IS_BONE                        = 0x800,
  IS_SKIN_COMPOUND_BONE          = 0x1000,
  UNUSED                         = 0x4000,
  DECAL                          = 0x8000,
  COLOR_HAS_FRAME_BLEND          = 0x10000,
  ANIM_FACIAL                    = 0x20000,
  ANIM_ROTATION_ONLY             = 0x40000,
  MORPHED_SHAPE                  = 0x80000,
  VISIBILITY_OCCLUDER            = 0x100000,
  FOG2_PORTAL                    = 0x200000,
  SKIN_DUAL_QUATERNION           = 0x400000,
  VISIBILITY_QUALIFY             = 0x800000,
  FP_MODEL                       = 0x1000000,
  DISASSEMBLE_TRANSP_OCCLUSION   = 0x2000000,
  OFF_SCORCH                     = 0x4000000,
}
