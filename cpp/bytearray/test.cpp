#include "ByteArray.h"

#include <iostream>
#include <assert.h>

void test1()
{
    ByteArray arr;
    assert(arr.size() == 0);
}

void test2()
{
    ByteArray arr1("BC02");
    assert(arr1.size() == 2);
    assert(arr1[0] == 0xBC);
    assert(arr1[1] == 0x02);
    std::string s = arr1.as_string();
    assert(s.length() == 4);
    assert(strcmp(s.c_str(), "BC02") == 0);
}

void test3()
{
    ByteArray arr1("BC02");
    ByteArray arr2("1234");
    arr1.append(arr2);
    assert(arr1.size() == 4);
    assert(arr1[0] == 0xBC);
    assert(arr1[1] == 0x02);
    assert(arr1[2] == 0x12);
    assert(arr1[3] == 0x34);
}

int main()
{
    test1();
    test2();
    test3();
    std::cout << "Unit test all passed." << std::endl;
    std::cout << "Press any key to quit ..."; std::cin.ignore();
    return 0;
}
