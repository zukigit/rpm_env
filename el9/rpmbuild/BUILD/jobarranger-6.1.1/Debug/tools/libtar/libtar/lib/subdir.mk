################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../tools/libtar/libtar/lib/append.c \
../tools/libtar/libtar/lib/block.c \
../tools/libtar/libtar/lib/decode.c \
../tools/libtar/libtar/lib/encode.c \
../tools/libtar/libtar/lib/extract.c \
../tools/libtar/libtar/lib/handle.c \
../tools/libtar/libtar/lib/output.c \
../tools/libtar/libtar/lib/util.c \
../tools/libtar/libtar/lib/wrapper.c 

OBJS += \
./tools/libtar/libtar/lib/append.o \
./tools/libtar/libtar/lib/block.o \
./tools/libtar/libtar/lib/decode.o \
./tools/libtar/libtar/lib/encode.o \
./tools/libtar/libtar/lib/extract.o \
./tools/libtar/libtar/lib/handle.o \
./tools/libtar/libtar/lib/output.o \
./tools/libtar/libtar/lib/util.o \
./tools/libtar/libtar/lib/wrapper.o 

C_DEPS += \
./tools/libtar/libtar/lib/append.d \
./tools/libtar/libtar/lib/block.d \
./tools/libtar/libtar/lib/decode.d \
./tools/libtar/libtar/lib/encode.d \
./tools/libtar/libtar/lib/extract.d \
./tools/libtar/libtar/lib/handle.d \
./tools/libtar/libtar/lib/output.d \
./tools/libtar/libtar/lib/util.d \
./tools/libtar/libtar/lib/wrapper.d 


# Each subdirectory must supply rules for building sources it contributes
tools/libtar/libtar/lib/%.o: ../tools/libtar/libtar/lib/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


