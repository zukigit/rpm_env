################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxcommshigh/comms.c 

OBJS += \
./src/libs/zbxcommshigh/comms.o 

C_DEPS += \
./src/libs/zbxcommshigh/comms.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxcommshigh/%.o: ../src/libs/zbxcommshigh/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


