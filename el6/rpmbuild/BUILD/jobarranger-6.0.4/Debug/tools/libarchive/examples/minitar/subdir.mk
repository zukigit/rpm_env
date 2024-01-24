################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../tools/libarchive/examples/minitar/minitar.c \
../tools/libarchive/examples/minitar/tree.c 

OBJS += \
./tools/libarchive/examples/minitar/minitar.o \
./tools/libarchive/examples/minitar/tree.o 

C_DEPS += \
./tools/libarchive/examples/minitar/minitar.d \
./tools/libarchive/examples/minitar/tree.d 


# Each subdirectory must supply rules for building sources it contributes
tools/libarchive/examples/minitar/%.o: ../tools/libarchive/examples/minitar/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


