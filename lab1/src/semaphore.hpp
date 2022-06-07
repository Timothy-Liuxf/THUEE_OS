#ifndef SEMAPHORE_HPP
#define SEMAPHORE_HPP

#include <semaphore.h>
#include <stdexcept>

class semaphore
{
public:
    semaphore(unsigned int init_val)
    {
        if (::sem_init(&_sem, 0, init_val) != 0) {
            throw std::runtime_error("Fail to create semaphore!");
        }
    }

    semaphore(const semaphore&) = delete;
    semaphore& operator=(const semaphore&) = delete;

    void
    wait()
    {
        if (::sem_wait(&_sem) != 0) {
            throw std::runtime_error("Fail to wait semaphore!");
        }
    }

    void
    post()
    {
        if (::sem_post(&_sem) != 0) {
            throw std::runtime_error("Fail to post semaphore!");
        }
    }

    int
    get_value() const
    {
        int ans;
        if (::sem_getvalue(const_cast<::sem_t*>(&_sem), &ans) != 0) {
            throw std::runtime_error("Fail to get the value of the semaphore!");
        }
        return ans;
    }

    ~semaphore() noexcept { ::sem_destroy(&_sem); }

private:
    ::sem_t _sem;
};

#endif // !SEMAPHORE_HPP
