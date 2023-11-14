################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxsysinfo/unknown/diskio.c \
../src/libs/zbxsysinfo/unknown/unknown.c 

OBJS += \
./src/libs/zbxsysinfo/unknown/diskio.o \
./src/libs/zbxsysinfo/unknown/unknown.o 

C_DEPS += \
./src/libs/zbxsysinfo/unknown/diskio.d \
./src/libs/zbxsysinfo/unknown/unknown.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxsysinfo/unknown/%.o: ../src/libs/zbxsysinfo/unknown/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


