################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxmemory/memalloc.c \
../src/libs/zbxmemory/strpool.c 

OBJS += \
./src/libs/zbxmemory/memalloc.o \
./src/libs/zbxmemory/strpool.o 

C_DEPS += \
./src/libs/zbxmemory/memalloc.d \
./src/libs/zbxmemory/strpool.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxmemory/%.o: ../src/libs/zbxmemory/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


