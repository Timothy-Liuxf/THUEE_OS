#ifndef CUSTOMER_HPP
#define CUSTOMER_HPP

#include <atomic>
#include <stdexcept>

#include "semaphore.hpp"

class customer
{
public:
    customer(int idx, int serv_time, int enter_time)
        : _idx(idx), _serv_time(serv_time), _enter_time(enter_time), _sem(0u)
    {
        if (serv_time < 0 || enter_time < 0) {
            throw std::invalid_argument("In constructor of customer: serv_time and enter_time should be non-negative!");
        }
    }
    customer(const customer&) = delete;
    customer& operator=(const customer&) = delete;

    void
    wait_sem()
    {
        _sem.wait();
    }

    void
    post_sem()
    {
        _sem.post();
    }

    int
    get_idx() const noexcept
    {
        return _idx;
    }

    int
    get_enter_time() const noexcept
    {
        return _enter_time;
    }

    int
    get_serv_time() const noexcept
    {
        return _serv_time;
    }

private:
    int       _idx;
    int       _serv_time;
    int       _enter_time;
    semaphore _sem;
};

#endif // !CUSTOMER_HPP
