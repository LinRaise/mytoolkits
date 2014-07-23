
#include "ByteArray.h"

#include <algorithm>

ByteArray::ByteArray(const std::string hex)
{
    assert(hex.length() % 2 == 0);
    std::size_t len = hex.length() / 2;
    for(std::size_t i = 0; i < len; i++)
    {
        m_vec.push_back( static_cast<BYTE> ((hex_char_to_int(hex[i * 2]) << 4)+ hex_char_to_int(hex[i * 2 + 1])) );
    }
}

const std::string LOOKUP = "0123456789ABCDEF";

std::string ByteArray::as_string() const
{
    std::vector<char> tmp;
    tmp.reserve(size() * 2);
    for( std::size_t i = 0, s = size(); i < s; i++ ) {
        tmp.push_back( LOOKUP[ m_vec[ i ] >> 4 ] );
        tmp.push_back( LOOKUP[ m_vec[ i ] & 0x0f ] );
    }
    return std::string(tmp.begin(), tmp.end());
}

std::size_t ByteArray::hex_char_to_int(const char input)
{
    std::size_t found = LOOKUP.find(toupper(input));
    if (found != std::string::npos)
    {
        return found;
    }
    throw std::string("Invalid hex char");
}

// following algorithm comes from: 
// http://stackoverflow.com/questions/10564491/function-to-calculate-a-crc16-checksum
#define CRC16 0x8005
UINT_16 gen_crc16(const BYTE *data, std::size_t size)
{
    UINT_16 out = 0;
    int bits_read = 0, bit_flag;

    /* Sanity check: */
    if(data == NULL)
        return 0;

    while(size > 0)
    {
        bit_flag = out >> 15;

        /* Get next bit: */
        out <<= 1;
        out |= (*data >> bits_read) & 1; // item a) work from the least significant bits

        /* Increment bit counter: */
        bits_read++;
        if(bits_read > 7)
        {
            bits_read = 0;
            data++;
            size--;
        }

        /* Cycle check: */
        if(bit_flag)
            out ^= CRC16;

    }

    // item b) "push out" the last 16 bits
    int i;
    for (i = 0; i < 16; ++i) {
        bit_flag = out >> 15;
        out <<= 1;
        if(bit_flag)
            out ^= CRC16;
    }

    // item c) reverse the bits
    UINT_16 crc = 0;
    i = 0x8000;
    int j = 0x0001;
    for (; i != 0; i >>=1, j <<= 1) {
        if (i & out) crc |= j;
    }

    return crc;
}

UINT_16 ByteArray::crc16() const
{
    return gen_crc16(&m_vec[0], m_vec.size());
}
