export LDLIBS += -pthread -lpthread
export CXXFLAGS ?= -O2 -Wall -Wpedantic -Wextra -std=c++11
export DEPS_DIR = deps
export BIN_DIR = bin
export TARGET_DIR = target

export LD = $(CXX) $(LDLIBS) $(LDFLAGS)
export CXX += $(CXXFLAGS)
export MKDIR ?= mkdir -p

DEPS_DIR_GUARD = $($(MKDIR) -p $(DEPS_DIR))

SRCS = $(wildcard *.cpp)
OBJS = $(patsubst %.cpp,$(BIN_DIR)/%.o, $(SRCS))
DEPS = $(patsubst %.cpp,$(DEPS_DIR)/%.d, $(SRCS))
TARGET = $(TARGET_DIR)/main

all: $(TARGET)

$(TARGET): $(OBJS) | $(TARGET_DIR)
	$(LD) -o $@ $^

$(OBJS): $(BIN_DIR)/%.o: %.cpp | $(BIN_DIR)
	$(CXX) -o $@ -c $<

$(DEPS): $(DEPS_DIR)/%.d: %.cpp | $(DEPS_DIR)
	$(CXX) -MM $< | sed -e 1's,^,$(BIN_DIR)/,' > $@

include $(DEPS)

$(DEPS_DIR):
	$(MKDIR) $(DEPS_DIR)
$(BIN_DIR):
	$(MKDIR) $(BIN_DIR)
$(TARGET_DIR):
	$(MKDIR) $(TARGET_DIR)

.PHONY: clean
clean:
	-$(RM) -r $(DEPS_DIR) $(BIN_DIR) $(TARGET_DIR)
