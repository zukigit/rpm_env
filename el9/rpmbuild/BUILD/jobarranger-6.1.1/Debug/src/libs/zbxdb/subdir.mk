################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxdb/db.c 

OBJS += \
./src/libs/zbxdb/db.o 

C_DEPS += \
./src/libs/zbxdb/db.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxdb/%.o: ../src/libs/zbxdb/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


