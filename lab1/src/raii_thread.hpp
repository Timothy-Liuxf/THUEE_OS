#ifndef RAII_THREAD_HPP
#define RAII_THREAD_HPP

#include <thread>
#include <utility>

class raii_thread
{
public:
    raii_thread() noexcept              = default;
    raii_thread(raii_thread&&) noexcept = default;
    raii_thread(const raii_thread&)     = delete;
    raii_thread& operator=(raii_thread&&) noexcept = default;
    raii_thread& operator=(const raii_thread&) noexcept = delete;

    template <typename Function, typename... Args>
    explicit raii_thread(Function&& f, Args&&... args) : _thrd(std::forward<Function>(f), std::forward<Args>(args)...)
    {
    }

    void
    join()
    {
        _thrd.join();
    }

    void
    detach()
    {
        _thrd.detach();
    }

    ~raii_thread()
    {
        if (_thrd.joinable()) {
            _thrd.join();
        }
    }

private:
    std::thread _thrd;
};

#endif // !RAII_THREAD_HPP
