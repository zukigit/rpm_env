################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxserver/evalfunc.c \
../src/libs/zbxserver/expression.c 

OBJS += \
./src/libs/zbxserver/evalfunc.o \
./src/libs/zbxserver/expression.o 

C_DEPS += \
./src/libs/zbxserver/evalfunc.d \
./src/libs/zbxserver/expression.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxserver/%.o: ../src/libs/zbxserver/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


