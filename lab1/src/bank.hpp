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
#include <utility>
#include <vector>

#include "customer.hpp"
#include "raii_thread.hpp"
#include "semaphore.hpp"
#include "timer.hpp"

class bank
{
private:
    using timer_t                  = timer<std::chrono::milliseconds>;
    static constexpr int time_zoom = 100;

public:
    bank(int nteller, const std::vector<std::unique_ptr<customer>>& customers) : _customer_sem(0u)
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
    }

    bank(const bank&) = delete;
    bank& operator=(const bank&) = delete;

private:
    void
    _customer_action(customer& self_customer)
    {
        timer_t::sleep(self_customer.get_enter_time() * time_zoom);

        // Enter bank

        {
            std::unique_lock<std::mutex> lock(_custumer_queue_mtx);
            _customer_queue.emplace(&self_customer);
            _customer_sem.post(); // V
        }
        std::printf("[Customer: %d] entered the bank at %d.\n", self_customer.get_idx(),
                    self_customer.get_enter_time()); // Use std::printf instead of std::cout because it's thread-safe

        // Wait for server end

        self_customer.wait_sem();

        // Quit bank

        std::printf("[Customer: %d] left the bank.\n", self_customer.get_idx());
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
            std::printf("[Teller: %d] started to serve [customer: %d] at time: %d\n", teller_idx, servee->get_idx(),
                        static_cast<int>(std::round(static_cast<double>(_timer.get_time()) / time_zoom)));
            timer_t::sleep(servee->get_serv_time() * time_zoom);
            servee->post_sem();
            std::printf("[Teller: %d] end serving [customer: %d] at time: %d\n", teller_idx, servee->get_idx(),
                        static_cast<int>(std::round(static_cast<double>(_timer.get_time()) / time_zoom)));
        }
    }

    semaphore             _customer_sem;
    timer_t               _timer;
    std::queue<customer*> _customer_queue;
    std::mutex            _custumer_queue_mtx;
};

#endif // !BANK_HPP
