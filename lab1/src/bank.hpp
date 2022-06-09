////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the THUEE_OS project.
//
// Copyright (C) 2022 Timothy-LiuXuefeng
//
// MIT License
//

#ifndef BANK_HPP
#define BANK_HPP

#include <atomic>
#include <cassert>
#include <cmath>
#include <cstdio>
#include <functional>
#include <memory>
#include <mutex>
#include <queue>
#include <string>
#include <type_traits>
#include <utility>
#include <vector>

#include "customer.hpp"
#include "raii_thread.hpp"
#include "semaphore.hpp"
#include "timer.hpp"

template <typename Judger>
class bank : private Judger // Use inheritance for EBO in case Judger may be empty
{
private:
    using timer_t                  = timer<std::chrono::milliseconds>;
    static constexpr int time_zoom = 200;

public:
    bank(int nteller, const std::vector<std::unique_ptr<customer>>& customers)
        : Judger(nteller, customers), _customer_sem(0u)
    {
        std::vector<raii_thread> customer_thrds;
        customer_thrds.reserve(customer_thrds.size());

        for (int i = 0; i < nteller; ++i) {
            raii_thread(&bank::_teller_action, this, i).detach();
        }

        for (int i = 0; i < (int)customers.size(); ++i) {
            customer_thrds.emplace_back(&bank::_customer_action, this, std::ref(*customers[i]));
        }

        for (auto& thrd : customer_thrds) {
            thrd.join();
        }

        if (Judger::judge()) {
            std::cout << "Test passed!" << std::endl;
        } else {
            std::cout << "Test failed!" << std::endl;
        }
    }

    bank(const bank&)            = delete;
    bank& operator=(const bank&) = delete;

private:
    void
    _customer_action(customer& self_customer)
    {
        timer_t::sleep(self_customer.get_enter_time() * time_zoom);

        // Enter bank

        Judger::enter_bank(self_customer.get_idx(), self_customer.get_enter_time());
        {
            std::unique_lock<std::mutex> lock(_custumer_queue_mtx);
            _customer_queue.emplace(&self_customer);
            _customer_sem.post(); // V
        }

        // Wait for server end

        self_customer.wait_sem();

        // Leave bank

        Judger::leave_bank(self_customer.get_idx(), _get_current_time());
    }

    void
    _teller_action(int teller_idx)
    {
        while (true) {

            // Wait for customer

            _customer_sem.wait();

            // Serve customer

            customer* servee = nullptr;
            {
                std::unique_lock<std::mutex> lock(_custumer_queue_mtx);
                servee = _customer_queue.front();
                assert(servee != nullptr);
                _customer_queue.pop();
            }
            Judger::start_serve(teller_idx, servee->get_idx(), _get_current_time());

            timer_t::sleep(servee->get_serv_time() * time_zoom);

            Judger::end_serve(teller_idx, servee->get_idx(), _get_current_time());
            servee->post_sem();
        }
    }

    int
    _get_current_time()
    {
        return static_cast<int>(std::round(static_cast<double>(_timer.get_time()) / time_zoom));
    }

    semaphore             _customer_sem;
    timer_t               _timer;
    std::queue<customer*> _customer_queue;
    std::mutex            _custumer_queue_mtx;
};

#endif // !BANK_HPP
