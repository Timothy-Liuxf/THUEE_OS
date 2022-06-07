#include <iostream>
#include <memory>
#include <sstream>
#include <string>
#include <vector>

#include "bank.hpp"

template <typename T, typename... Args>
std::unique_ptr<T>
cxx11_make_unique(Args... args)
{
    return std::unique_ptr<T>(new T(std::forward<Args>(args)...));
}

int
main()
{
    std::vector<std::unique_ptr<customer>> customers;
    std::string                            linebuf;
    int                                    nline   = 0;
    constexpr int                          nteller = 2;
    std::istringstream                     sin;
    while (std::getline(std::cin, linebuf)) {
        if (!std::cin) {
            break;
        }

        int idx, enter, serv;
        sin.str(linebuf);
        sin >> idx >> enter >> serv;
        if (!sin) {
            std::cerr << "Input format incorrect at line: " << nline << std::endl;
            return -1;
        }
        sin.clear();
        customers.emplace_back(cxx11_make_unique<customer>(idx, serv, enter));
        ++nline;
    }

    bank(nteller, customers);
    return 0;
}
