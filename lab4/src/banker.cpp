#include "banker.hpp"
#include "arithmetic.hpp"

#include <cstdio>
#include <queue>
#include <stdexcept>

#define SAFE_LOG(...) std::printf(__VA_ARGS__)

bool
is_safe_state(std::vector<std::vector<int>> need, std::vector<std::vector<int>> allocated, std::vector<int> available)
{
    if (need.size() == 0) { // Trivial circumstance, safe
        return true;
    }

    std::queue<std::size_t> left_process;

    // Push all processes into the queue

    for (std::size_t i = 0; i < need.size(); ++i) {
        left_process.emplace(i);
    }

    do {

        // Check for once

        bool        has_allocated = false;
        std::size_t left_count    = left_process.size();

        for (std::size_t i = 0; i < left_count; ++i) {
            auto id = left_process.front();
            left_process.pop();

            // Check if process 'pid' can satisfy

            bool satisfy = true;
            for (std::size_t j = 0; j < available.size(); ++j) {
                if (available[j] < need[id][j]) {
                    satisfy = false;
                    break;
                }
            }

            if (satisfy) { // Can finish task, allocate to it.
                available += allocated[id];
                SAFE_LOG("In safe check: try to allocate to: %d\n", (int)id);
                has_allocated = true;
            } else { // Cannot finish task, push back the process.
                left_process.emplace(id);
            }
        }

        if (!has_allocated) { // No process allocated
            return false;
        }
    } while (!left_process.empty());
    return true;
}

allocate_result
can_allocate(const std::vector<std::vector<int>>& need, const std::vector<std::vector<int>>& allocated,
             const std::vector<int>& available, const std::vector<int>& acquire, std::size_t id)
{
    if (id >= need.size()) {
        throw std::invalid_argument("Process ID is invalid!");
    }

    for (std::size_t i = 0; i < acquire.size(); ++i) {
        if (acquire[i] > need[id][i]) {
            SAFE_LOG("Fail to allocate to %d\n", (int)id);
            return allocate_result::FAIL;
        }
    }

    for (std::size_t i = 0; i < acquire.size(); ++i) {
        if (acquire[i] > available[i]) {
            SAFE_LOG("Process: %d begins to wait\n", (int)id);
            return allocate_result::WAIT;
        }
    }

    auto new_need = need;
    new_need[id] -= acquire;
    auto new_available = available - acquire;
    auto new_allocated = allocated;
    new_allocated[id] += acquire;

    if (is_safe_state(std::move(new_need), std::move(new_allocated), std::move(new_available))) {
        SAFE_LOG("Success to allocate to %d\n", (int)id);
        return allocate_result::SUCCESS;
    }
    SAFE_LOG("Process: %d begins to wait\n", (int)id);
    return allocate_result::WAIT;
}
