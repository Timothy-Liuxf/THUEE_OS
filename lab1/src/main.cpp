#include <algorithm>
#include <cmath>
#include <iomanip>
#include <iostream>
#include <list>
#include <map>
#include <memory>
#include <mutex>
#include <sstream>
#include <string>
#include <unordered_map>
#include <utility>
#include <vector>

#include "bank.hpp"

class judger
{
private:
    struct _event
    {
        enum class _event_type_t
        {
            none        = 0,
            enter_bank  = 1,
            leave_bank  = 2,
            start_serve = 3,
            end_serve   = 4
        } event_type;

        int arg0;
        int arg1;
        int arg2;

        _event(_event_type_t event_type = _event_type_t::none, int arg0 = 0, int arg1 = 0, int arg2 = 0)
            : event_type(event_type), arg0(arg0), arg1(arg1), arg2(arg2)
        {
        }
    };
    using _event_type_t = _event::_event_type_t;

public:
    judger(int nteller, const std::vector<std::unique_ptr<customer>>& customers) : _nteller(nteller)
    {
        for (auto&& cust_ptr : customers) {
            _customer_db[cust_ptr->get_idx()] = std::make_pair(cust_ptr->get_enter_time(), cust_ptr->get_serv_time());
        }
    }

    void
    enter_bank(int customer_idx, int time)
    {
        std::unique_lock<std::mutex> lock(_mtx);
        std::printf("[Customer: %d] entered the bank at %d.\n", customer_idx, time);
        _event_list.emplace_back(_event_type_t::enter_bank, customer_idx, time);
    }

    void
    leave_bank(int customer_idx, int time)
    {
        std::unique_lock<std::mutex> lock(_mtx);
        std::printf("[Customer: %d] left the bank at: %d.\n", customer_idx, time);
        _event_list.emplace_back(_event_type_t::leave_bank, customer_idx, time);
    }

    void
    start_serve(int teller_idx, int customer_idx, int time)
    {
        std::unique_lock<std::mutex> lock(_mtx);
        std::printf("[Teller: %d] started to serve [customer: %d] at time: %d\n", teller_idx, customer_idx, time);
        _event_list.emplace_back(_event_type_t::start_serve, teller_idx, customer_idx, time);
    }

    void
    end_serve(int teller_idx, int customer_idx, int time)
    {
        std::unique_lock<std::mutex> lock(_mtx);
        std::printf("[Teller: %d] end serving [customer: %d] at time: %d\n", teller_idx, customer_idx, time);
        _event_list.emplace_back(_event_type_t::end_serve, teller_idx, customer_idx, time);
    }

    bool
    judge(const bool display = true) const
    {
        std::unique_lock<std::mutex> lock(_mtx);
        std::vector<int>             teller_end_time(_nteller, 0);
        std::unordered_map<int, int> customer_start_time;
        std::unordered_map<int, int> customer_end_time;
        std::unordered_map<int, int> customer_enter_time;
        std::unordered_map<int, int> customer_leave_time;
        std::unordered_map<int, int> customer_serv_teller;

        bool                         result = true;

        for (auto event : _event_list) {
            switch (event.event_type) {
            default:
            case _event_type_t::none:
                if (!display) return false;
                result = false;
                std::cerr << " [ERROR] Unknown event!\n";
                break;
            case _event_type_t::start_serve: {
                int  teller_idx    = event.arg0;
                int  customer_idx  = event.arg1;
                int  time          = event.arg2;
                auto customer_data = _customer_db.find(customer_idx);
                if (customer_data == _customer_db.end()) {
                    if (!display) return false;
                    result = false;
                    std::cerr << "[ERROR] Unknown customer: " << customer_idx << "\n";
                } else {
                    if (customer_start_time.find(customer_idx) != customer_start_time.end()) {
                        if (!display) return false;
                        result = false;
                        std::cerr << "[ERROR] [Customer: " << customer_idx << "] has been served!\n";
                    }
                    if (teller_end_time[teller_idx] > time) {
                        if (!display) return false;
                        result = false;
                        std::cerr << "[ERROR] [Teller: " << teller_idx << "] is serving another customer!\n";
                    }
                    if (time < customer_data->second.first) {
                        if (!display) return false;
                        result = false;
                        std::cerr << "[ERROR] [Customer: " << customer_idx << "] is served before entering\n";
                    }
                    customer_start_time[customer_idx]  = time;
                    customer_serv_teller[customer_idx] = teller_idx;
                    teller_end_time[teller_idx]        = time + customer_data->second.second;
                }
                break;
            }
            case _event_type_t::end_serve: {
                int  teller_idx    = event.arg0;
                int  customer_idx  = event.arg1;
                int  time          = event.arg2;
                auto customer_data = _customer_db.find(customer_idx);
                if (customer_data == _customer_db.end()) {
                    if (!display) return false;
                    result = false;
                    std::cerr << "[ERROR] Unknown customer: " << customer_idx << "\n";
                } else {
                    if (customer_start_time.find(customer_idx) == customer_start_time.end()) {
                        if (!display) return false;
                        result = false;
                        std::cerr << "[ERROR] [Customer: " << customer_idx << "] hasn't started!\n";
                    }
                    if (teller_end_time[teller_idx] != time) {
                        if (std::abs(teller_end_time[teller_idx] - time) <= 2) {
                            std::cerr << "[WARNING] [Teller: " << teller_idx
                                      << "] didn't serve the time as [Customer: " << customer_idx
                                      << "] expected exactly!\n";
                        } else {
                            if (!display) return false;
                            result = false;
                            std::cerr << "[ERROR] [Teller: " << teller_idx
                                      << "] didn't serve correct time as [Customer: " << customer_idx
                                      << "] expected!\n";
                        }
                    }
                    customer_end_time[customer_idx] = time;
                }
                break;
            }
            case _event_type_t::enter_bank: {
                int  customer_idx  = event.arg0;
                int  time          = event.arg1;
                auto customer_data = _customer_db.find(customer_idx);
                if (customer_data == _customer_db.end()) {
                    if (!display) return false;
                    result = false;
                    std::cerr << "[ERROR] Unknown customer: " << customer_idx << "\n";
                } else {
                    if (time != customer_data->second.first) {
                        if (!display) return false;
                        result = false;
                        std::cerr << "[ERROR] [Customer: " << customer_idx << "] enter time is incorrect!\n";
                    }
                    customer_enter_time[customer_idx] = time;
                }
                break;
            }
            case _event_type_t::leave_bank: {
                int  customer_idx  = event.arg0;
                int  time          = event.arg1;
                auto customer_data = _customer_db.find(customer_idx);
                if (customer_data == _customer_db.end()) {
                    if (!display) return false;
                    result = false;
                    std::cerr << "[ERROR] Unknown customer: " << customer_idx << "\n";
                } else {
                    auto itr = customer_end_time.find(customer_idx);
                    if (itr == customer_end_time.end()) {
                        if (!display) return false;
                        result = false;
                        std::cerr << "[ERROR] [Customer: " << customer_idx << "] left bank before ending\n";
                    }
                    if (time != itr->second) {
                        if (!display) return false;
                        result = false;
                        std::cerr << "[ERROR] [Customer: " << customer_idx << "] left bank not just after ending\n";
                    }
                    customer_leave_time[customer_idx] = time;
                }
                break;
            }
            }
        }

        for (auto itr = _customer_db.begin(); itr != _customer_db.end(); ++itr) {
            int idx = itr->first;
            if (customer_start_time.find(idx) == customer_start_time.end()
                || customer_end_time.find(idx) == customer_end_time.end()
                || customer_enter_time.find(idx) == customer_enter_time.end()
                || customer_leave_time.find(idx) == customer_leave_time.end()
                || customer_serv_teller.find(idx) == customer_serv_teller.end()) {
                if (!display) return false;
                result = false;
                std::cerr << "[ERROR] [Customer: " << idx << "] didn't been served!\n";
            }
        }

        if (display) {
            for (auto itr = _customer_db.begin(); itr != _customer_db.end(); ++itr) {
                int idx = itr->first;
                std::cout << std::right << std::setw(4) << idx << ": ";

                if (customer_start_time.find(idx) == customer_start_time.end()
                    || customer_end_time.find(idx) == customer_end_time.end()
                    || customer_enter_time.find(idx) == customer_enter_time.end()
                    || customer_leave_time.find(idx) == customer_leave_time.end()
                    || customer_serv_teller.find(idx) == customer_serv_teller.end()) {
                    continue;
                }

                std::cout << std::left << std::setw(4) << customer_serv_teller[idx];

                int tmp = std::min(customer_enter_time[idx], customer_start_time[idx]);
                int i   = 0;
                for (; i < tmp; ++i) {
                    std::cout << '.';
                }
                for (; i < customer_start_time[idx]; ++i) {
                    std::cout.put('-');
                }
                for (; i < customer_end_time[idx]; ++i) {
                    std::cout.put('*');
                }
                for (; i < customer_leave_time[idx]; ++i) {
                    std::cout.put('E'); // ERROR! 'E' cannot appear normally!
                }
                endl(std::cout);
            }
        }

        return result;
    }

private:
    int                                _nteller;
    std::map<int, std::pair<int, int>> _customer_db;
    std::list<_event>                  _event_list;
    mutable std::mutex                 _mtx;
};

template <typename T, typename... Args>
std::unique_ptr<T>
cxx11_make_unique(Args... args)
{
    return std::unique_ptr<T>(new T(std::forward<Args>(args)...));
}

int
main(int argc, char* argv[])
{
    std::vector<std::unique_ptr<customer>> customers;
    std::string                            linebuf;
    int                                    nline   = 0;
    int                                    nteller = 3;
    std::istringstream                     sin;

    if (argc > 1) {
        try {
            int tmp = std::stoi(std::string(argv[1]));
            if (tmp > 0) {
                nteller = tmp;
            }
        } catch (...) {
        }
    }

    std::cout << "Number of teller: " << nteller << std::endl;

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

    bank<judger>(nteller, customers);
    return 0;
}
