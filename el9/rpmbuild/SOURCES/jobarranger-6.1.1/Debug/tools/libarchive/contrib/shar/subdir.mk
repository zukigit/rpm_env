################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../tools/libarchive/contrib/shar/shar.c \
../tools/libarchive/contrib/shar/tree.c 

OBJS += \
./tools/libarchive/contrib/shar/shar.o \
./tools/libarchive/contrib/shar/tree.o 

C_DEPS += \
./tools/libarchive/contrib/shar/shar.d \
./tools/libarchive/contrib/shar/tree.d 


# Each subdirectory must supply rules for building sources it contributes
tools/libarchive/contrib/shar/%.o: ../tools/libarchive/contrib/shar/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


