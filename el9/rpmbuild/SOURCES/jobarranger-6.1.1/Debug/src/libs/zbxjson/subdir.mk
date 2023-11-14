################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxjson/json.c 

OBJS += \
./src/libs/zbxjson/json.o 

C_DEPS += \
./src/libs/zbxjson/json.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxjson/%.o: ../src/libs/zbxjson/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


