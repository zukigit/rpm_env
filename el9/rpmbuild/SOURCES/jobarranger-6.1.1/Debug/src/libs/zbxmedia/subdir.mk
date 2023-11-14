################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxmedia/email.c \
../src/libs/zbxmedia/eztexting.c \
../src/libs/zbxmedia/jabber.c \
../src/libs/zbxmedia/sms.c 

OBJS += \
./src/libs/zbxmedia/email.o \
./src/libs/zbxmedia/eztexting.o \
./src/libs/zbxmedia/jabber.o \
./src/libs/zbxmedia/sms.o 

C_DEPS += \
./src/libs/zbxmedia/email.d \
./src/libs/zbxmedia/eztexting.d \
./src/libs/zbxmedia/jabber.d \
./src/libs/zbxmedia/sms.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxmedia/%.o: ../src/libs/zbxmedia/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


