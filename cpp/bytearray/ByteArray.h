

#ifndef ZYXF_BYTEARRAY_H
#define ZYXF_BYTEARRAY_H

#include "define.h"

#include <string>
#include <vector>
#include <assert.h>

class ByteArray
{
public:
    ByteArray() {}
    ByteArray(const BYTE* data, const std::size_t len) : m_vec(data, data + len) {}
    ByteArray(const ByteArray& array) : m_vec(array.m_vec) {}
    ByteArray(const std::string hex);
    ~ByteArray()                                        { clear(); }
    
    void clear()                                        { m_vec.clear(); }
    bool empty() const                                  { return m_vec.empty(); }
    const BYTE* data() const                            { return &m_vec[0]; }
    std::size_t size() const                            { return m_vec.size(); }
    std::string as_string() const;
    const BYTE operator[](const std::size_t i) const    { return m_vec[i]; }
    std::size_t word_size() const                       { return align(size(), 2) / 2; }
    std::size_t dword_size() const                      { return align(size(), 4) / 4; }
    ByteArray& append(const ByteArray& data)            { m_vec.insert(m_vec.end(), data.m_vec.begin(), data.m_vec.end()); return *this; }
    UINT_16 crc16() const;

    /* only to be used to align on a power of 2 boundary */
    static std::size_t align(std::size_t size, std::size_t boundary) 
                                                        { return ((size / boundary) + ((size % boundary == 0) ? 0 : 1)) * boundary; }

private:
    static std::size_t hex_char_to_int(const char input);
    
    std::vector<BYTE> m_vec;
};

#endif // ZYXF_BYTEARRAY_H
