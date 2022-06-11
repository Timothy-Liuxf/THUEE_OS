#include "arithmetic.hpp"
#include "banker.hpp"

#include <algorithm>
#include <iostream>

int
main()
{
    int ndata = 0;
    std::cin >> ndata;
    for (int t = 0; t < ndata; ++t) {
        int nprocess, nres;
        std::cin >> nprocess >> nres;
        std::vector<std::vector<int>> need(nprocess, std::vector<int>(nres));
        std::vector<std::vector<int>> allocated(nprocess, std::vector<int>(nres));
        std::vector<int>              available(nres);
        std::size_t                   acquire_id;
        std::vector<int>              acquire(nres);

        auto                          inputMatrix = [](std::vector<std::vector<int>>& mat)
        {
            std::for_each(mat.begin(), mat.end(),
                          [](std::vector<int>& row)
                          { std::for_each(row.begin(), row.end(), [](int& n) { std::cin >> n; }); });
        };
        auto inputVec = [](std::vector<int>& vec)
        { std::for_each(vec.begin(), vec.end(), [](int& n) { std::cin >> n; }); };

        inputMatrix(need);
        inputMatrix(allocated);
        inputVec(available);
        std::cin >> acquire_id;
        inputVec(acquire);

        if (!std::cin) {
            std::cerr << "Input failed!\n";
            return 1;
        }

        auto outputMatrix = [](std::vector<std::vector<int>>& mat)
        {
            std::for_each(mat.begin(), mat.end(),
                          [](std::vector<int>& row)
                          {
                              std::for_each(row.begin(), row.end(), [](int& n) { std::cout << n << ' '; });
                              endl(std::cout);
                          });
        };
        auto outputVec = [](std::vector<int>& vec)
        {
            std::for_each(vec.begin(), vec.end(), [](int& n) { std::cout << n << ' '; });
            endl(std::cout);
        };

        switch (can_allocate(need, allocated, available, acquire, acquire_id)) {
        case allocate_result::SUCCESS:
            need[acquire_id] -= acquire;
            allocated[acquire_id] += acquire;
            available -= acquire;
            break;
        default:
            break;
        }

        // Output resources

        endl(std::cout);
        std::cout << "Allocated matrix: \n";
        outputMatrix(allocated);
        endl(std::cout);
        std::cout << "Need matrix: \n";
        outputMatrix(need);
        endl(std::cout);
        std::cout << "Available vector: \n";
        outputVec(available);
        endl(std::cout);

        std::cout << "==========\n" << std::endl;
    }
    return 0;
}
