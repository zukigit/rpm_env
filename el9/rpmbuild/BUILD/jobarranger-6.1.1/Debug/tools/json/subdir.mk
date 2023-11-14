################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../tools/json/arraylist.c \
../tools/json/debug.c \
../tools/json/json_object.c \
../tools/json/json_tokener.c \
../tools/json/json_util.c \
../tools/json/linkhash.c \
../tools/json/printbuf.c \
../tools/json/test1.c \
../tools/json/test2.c \
../tools/json/test3.c 

OBJS += \
./tools/json/arraylist.o \
./tools/json/debug.o \
./tools/json/json_object.o \
./tools/json/json_tokener.o \
./tools/json/json_util.o \
./tools/json/linkhash.o \
./tools/json/printbuf.o \
./tools/json/test1.o \
./tools/json/test2.o \
./tools/json/test3.o 

C_DEPS += \
./tools/json/arraylist.d \
./tools/json/debug.d \
./tools/json/json_object.d \
./tools/json/json_tokener.d \
./tools/json/json_util.d \
./tools/json/linkhash.d \
./tools/json/printbuf.d \
./tools/json/test1.d \
./tools/json/test2.d \
./tools/json/test3.d 


# Each subdirectory must supply rules for building sources it contributes
tools/json/%.o: ../tools/json/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


