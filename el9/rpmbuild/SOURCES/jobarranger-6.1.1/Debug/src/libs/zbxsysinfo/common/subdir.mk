################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxsysinfo/common/common.c \
../src/libs/zbxsysinfo/common/file.c \
../src/libs/zbxsysinfo/common/http.c \
../src/libs/zbxsysinfo/common/net.c \
../src/libs/zbxsysinfo/common/system.c 

OBJS += \
./src/libs/zbxsysinfo/common/common.o \
./src/libs/zbxsysinfo/common/file.o \
./src/libs/zbxsysinfo/common/http.o \
./src/libs/zbxsysinfo/common/net.o \
./src/libs/zbxsysinfo/common/system.o 

C_DEPS += \
./src/libs/zbxsysinfo/common/common.d \
./src/libs/zbxsysinfo/common/file.d \
./src/libs/zbxsysinfo/common/http.d \
./src/libs/zbxsysinfo/common/net.d \
./src/libs/zbxsysinfo/common/system.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxsysinfo/common/%.o: ../src/libs/zbxsysinfo/common/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


