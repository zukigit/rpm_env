################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/jobarg_joblogput/jobarg_joblogput.c 

OBJS += \
./src/jobarg_joblogput/jobarg_joblogput.o 

C_DEPS += \
./src/jobarg_joblogput/jobarg_joblogput.d 


# Each subdirectory must supply rules for building sources it contributes
src/jobarg_joblogput/%.o: ../src/jobarg_joblogput/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


