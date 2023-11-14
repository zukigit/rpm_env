################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxwin32/perfmon.c \
../src/libs/zbxwin32/service.c 

OBJS += \
./src/libs/zbxwin32/perfmon.o \
./src/libs/zbxwin32/service.o 

C_DEPS += \
./src/libs/zbxwin32/perfmon.d \
./src/libs/zbxwin32/service.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxwin32/%.o: ../src/libs/zbxwin32/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


