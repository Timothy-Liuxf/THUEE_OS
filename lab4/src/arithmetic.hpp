#ifndef ARITHMETIC_HPP
#define ARITHMETIC_HPP

#include <stdexcept>
#include <vector>

template <typename T>
std::vector<T>
operator+(const std::vector<T>& v1, const std::vector<T>& v2)
{
    if (v1.size() != v2.size()) {
        throw std::invalid_argument("Two addable vectors must have the same size!");
    }

    std::vector<T> ans;
    ans.reserve(v1.size());

    for (std::size_t i = 0; i < v1.size(); ++i) {
        ans.emplace_back(v1[i] + v2[i]);
    }

    return ans;
}

template <typename T>
std::vector<T>
operator-(const std::vector<T>& v1, const std::vector<T>& v2)
{
    if (v1.size() != v2.size()) {
        throw std::invalid_argument("Two subable vectors must have the same size!");
    }

    std::vector<T> ans;
    ans.reserve(v1.size());

    for (std::size_t i = 0; i < v1.size(); ++i) {
        ans.emplace_back(v1[i] - v2[i]);
    }

    return ans;
}

template <typename T>
std::vector<T>&
operator+=(std::vector<T>& v1, const std::vector<T>& v2)
{
    if (v1.size() != v2.size()) {
        throw std::invalid_argument("Two addable vectors must have the same size!");
    }

    for (std::size_t i = 0; i < v1.size(); ++i) {
        v1[i] += v2[i];
    }

    return v1;
}

template <typename T>
std::vector<T>&
operator-=(std::vector<T>& v1, const std::vector<T>& v2)
{
    if (v1.size() != v2.size()) {
        throw std::invalid_argument("Two subable vectors must have the same size!");
    }

    for (std::size_t i = 0; i < v1.size(); ++i) {
        v1[i] -= v2[i];
    }

    return v1;
}

#endif // !ARITHMETIC_HPP
