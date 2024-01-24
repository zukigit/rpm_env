################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxalgo/algodefs.c \
../src/libs/zbxalgo/binaryheap.c \
../src/libs/zbxalgo/hashmap.c \
../src/libs/zbxalgo/hashset.c \
../src/libs/zbxalgo/vector.c 

OBJS += \
./src/libs/zbxalgo/algodefs.o \
./src/libs/zbxalgo/binaryheap.o \
./src/libs/zbxalgo/hashmap.o \
./src/libs/zbxalgo/hashset.o \
./src/libs/zbxalgo/vector.o 

C_DEPS += \
./src/libs/zbxalgo/algodefs.d \
./src/libs/zbxalgo/binaryheap.d \
./src/libs/zbxalgo/hashmap.d \
./src/libs/zbxalgo/hashset.d \
./src/libs/zbxalgo/vector.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxalgo/%.o: ../src/libs/zbxalgo/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


