
#ifndef ZYXF_DEFINE_H
#define ZYXF_DEFINE_H

typedef unsigned char       UINT_8;

#if defined(_MSC_VER)
typedef __int16             INT_16;
typedef unsigned __int16    UINT_16;
typedef __int32             INT_32;
typedef unsigned __int32    UINT_32;
typedef __int64             INT_64;
typedef unsigned __int64    UINT_64;
#else
#include <stdint.h>
typedef int16_t             INT_16;
typedef uint16_t            UINT_16;
typedef int32_t             INT_32;
typedef uint32_t            UINT_32;
typedef int64_t             INT_64;
typedef uint64_t            UINT_64;
#endif

#ifndef BYTE
typedef unsigned char       BYTE;
#endif

#endif // ZYXF_DEFINE_H
