################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxsys/mutexs.c \
../src/libs/zbxsys/symbols.c \
../src/libs/zbxsys/threads.c 

OBJS += \
./src/libs/zbxsys/mutexs.o \
./src/libs/zbxsys/symbols.o \
./src/libs/zbxsys/threads.o 

C_DEPS += \
./src/libs/zbxsys/mutexs.d \
./src/libs/zbxsys/symbols.d \
./src/libs/zbxsys/threads.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxsys/%.o: ../src/libs/zbxsys/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


