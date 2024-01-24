################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../tools/sqlite3/tea/generic/tclsqlite3.c 

OBJS += \
./tools/sqlite3/tea/generic/tclsqlite3.o 

C_DEPS += \
./tools/sqlite3/tea/generic/tclsqlite3.d 


# Each subdirectory must supply rules for building sources it contributes
tools/sqlite3/tea/generic/%.o: ../tools/sqlite3/tea/generic/%.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross GCC Compiler'
	gcc -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


