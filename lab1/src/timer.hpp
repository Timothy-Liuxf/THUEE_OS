#ifndef TIMER_HPP
#define TIMER_HPP

#include <chrono>
#include <thread>

template <typename TimeUnit>
class timer
{
public:
    using time_unit = TimeUnit;
    using time_t    = typename TimeUnit::rep;

    timer() : _start_time(_raw_time()) {}

    timer(const timer&) = delete;
    timer& operator=(const timer&) = delete;

    time_t
    get_time() const
    {
        return _raw_time() - _start_time;
    }

    void
    reset()
    {
        _start_time = _raw_time();
    }

    static void
    sleep(time_t time)
    {
        std::this_thread::sleep_for(time_unit(time));
    }

private:
    static time_t
    _raw_time()
    {
        return std::chrono::duration_cast<time_unit>(std::chrono::system_clock::now().time_since_epoch()).count();
    }

    time_t _start_time;
};

#endif // !TIMER_HPP
