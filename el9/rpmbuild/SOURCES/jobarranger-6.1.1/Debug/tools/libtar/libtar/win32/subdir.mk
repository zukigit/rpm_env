################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../tools/libtar/libtar/win32/dirent.c 

OBJS += \
./tools/libtar/libtar/win32/dirent.o 

C_DEPS += \
./tools/libtar/libtar/win32/dirent.d 


# Each subdirectory must supply rules for building sources it contributes
tools/libtar/libtar/win32/%.o: ../tools/libtar/libtar/win32/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


