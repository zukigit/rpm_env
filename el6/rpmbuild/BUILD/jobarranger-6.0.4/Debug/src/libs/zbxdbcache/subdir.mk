################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/libs/zbxdbcache/dbcache.c \
../src/libs/zbxdbcache/dbconfig.c \
../src/libs/zbxdbcache/nextchecks.c 

OBJS += \
./src/libs/zbxdbcache/dbcache.o \
./src/libs/zbxdbcache/dbconfig.o \
./src/libs/zbxdbcache/nextchecks.o 

C_DEPS += \
./src/libs/zbxdbcache/dbcache.d \
./src/libs/zbxdbcache/dbconfig.d \
./src/libs/zbxdbcache/nextchecks.d 


# Each subdirectory must supply rules for building sources it contributes
src/libs/zbxdbcache/%.o: ../src/libs/zbxdbcache/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


