################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../tools/libtar/alttar/getopt.c \
../tools/libtar/alttar/libtar.c 

OBJS += \
./tools/libtar/alttar/getopt.o \
./tools/libtar/alttar/libtar.o 

C_DEPS += \
./tools/libtar/alttar/getopt.d \
./tools/libtar/alttar/libtar.d 


# Each subdirectory must supply rules for building sources it contributes
tools/libtar/alttar/%.o: ../tools/libtar/alttar/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


