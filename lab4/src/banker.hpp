////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the THUEE_OS project.
//
// Copyright (C) 2022 Timothy-LiuXuefeng
//
// MIT License
//

#ifndef BANKER_HPP
#define BANKER_HPP

#include <vector>

enum class allocate_result
{
    FAIL    = 0,
    WAIT    = 1,
    SUCCESS = 2
};

bool            is_safe_state(std::vector<std::vector<int>> need, std::vector<std::vector<int>> allocated,
                              std::vector<int> available);

allocate_result can_allocate(const std::vector<std::vector<int>>& need, const std::vector<std::vector<int>>& allocated,
                             const std::vector<int>& available, const std::vector<int>& acquire, std::size_t id);

#endif // !BANKER_HPP
