################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxsysinfo/simple/ntp.c \
../src/libs/zbxsysinfo/simple/simple.c 

OBJS += \
./src/libs/zbxsysinfo/simple/ntp.o \
./src/libs/zbxsysinfo/simple/simple.o 

C_DEPS += \
./src/libs/zbxsysinfo/simple/ntp.d \
./src/libs/zbxsysinfo/simple/simple.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxsysinfo/simple/%.o: ../src/libs/zbxsysinfo/simple/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


