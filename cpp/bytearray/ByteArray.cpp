
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

UINT_16 ByteArray::crc16() const
{
    return 0;
}

UINT_32 ByteArray::crc32() const
{
    return 0;
}
