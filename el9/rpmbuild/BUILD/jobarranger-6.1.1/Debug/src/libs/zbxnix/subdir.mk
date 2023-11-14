################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxnix/daemon.c \
../src/libs/zbxnix/fatal.c \
../src/libs/zbxnix/ipc.c \
../src/libs/zbxnix/pid.c 

OBJS += \
./src/libs/zbxnix/daemon.o \
./src/libs/zbxnix/fatal.o \
./src/libs/zbxnix/ipc.o \
./src/libs/zbxnix/pid.o 

C_DEPS += \
./src/libs/zbxnix/daemon.d \
./src/libs/zbxnix/fatal.d \
./src/libs/zbxnix/ipc.d \
./src/libs/zbxnix/pid.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxnix/%.o: ../src/libs/zbxnix/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


