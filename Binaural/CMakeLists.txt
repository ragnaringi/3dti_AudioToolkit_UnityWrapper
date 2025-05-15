# cmake -DCMAKE_BUILD_TYPE=Release -B build
# cmake --build build
# Android Build Command
# cmake -DCMAKE_TOOLCHAIN_FILE=/Applications/Unity/Hub/Editor/6000.0.44f1/PlaybackEngines/AndroidPlayer/NDK/build/cmake/android.toolchain.cmake \
#       -DANDROID_ABI=arm64-v8a \
#       -DANDROID_PLATFORM=android-21 \
#       -B build -DCMAKE_BUILD_TYPE=Release

cmake_minimum_required(VERSION 3.15)
project(AudioPlugin3DTIToolkit LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF) 
set(LIB_TYPE MODULE)
set(CMAKE_OSX_DEPLOYMENT_TARGET "10.13" CACHE STRING "Minimum macOS deployment version")

option(USE_BRT_LIBRARY "Use BRT library" OFF)

if(APPLE)
    set(CMAKE_OSX_ARCHITECTURES "x86_64;arm64" CACHE INTERNAL "")
    if(CMAKE_SYSTEM_NAME STREQUAL "iOS")
        set(LIB_TYPE STATIC)
    endif()
endif()

# Collect your project sources
file(GLOB_RECURSE PROJECT_SRC CONFIGURE_DEPENDS src/*.cpp src/*.h)
# Remove files ending in HL.cpp or HA.cpp
foreach(file ${PROJECT_SRC})
    if(file MATCHES ".*HL\\.cpp$" OR file MATCHES ".*HA\\.cpp$")
        list(REMOVE_ITEM PROJECT_SRC "${file}")
    endif()
endforeach()
source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/src" PREFIX "src" FILES ${PROJECT_SRC})

# Collect third-party sources
file(GLOB_RECURSE 3DTI_TOOLKIT 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit/*.cpp" 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit/*.h"
)
source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit" PREFIX "3dti_Toolkit" FILES ${3DTI_TOOLKIT})

file(GLOB_RECURSE 3DTI_MODULES 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/BRIR/*.cpp" 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/BRIR/*.h"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/HRTF/*.cpp" 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/HRTF/*.h"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/ILD/*.cpp" 
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/ILD/*.h"
)
if(NOT WIN32)
    foreach(file ${3DTI_MODULES})
        if(file MATCHES ".*Factory\\.(cpp|h)$")
            list(REMOVE_ITEM 3DTI_MODULES ${file})
        endif()
    endforeach()
endif()
source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager" PREFIX "3dti_Modules" FILES ${3DTI_MODULES})

# Only add SOFA files for non Apple platforms
if(NEVER)
    file(GLOB_RECURSE SOFA 
        "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/src/*.cpp" 
        "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/src/*.h"
    )
    foreach(file ${SOFA})
        get_filename_component(fname ${file} NAME)
        if(NOT fname MATCHES "^SOFA")
            list(REMOVE_ITEM SOFA ${file})
        endif()
    endforeach()
    source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/src" PREFIX "SOFA" FILES ${SOFA})

    file(GLOB_RECURSE NETCDF
        "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/include/*.cpp" 
        "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/include/*.h"
    )
    source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/include" PREFIX "netcdf" FILES ${NETCDF})

    # Link library depenencies
    # file(GLOB LIBS "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/lib/win/*.lib")
    # target_link_libraries(AudioPlugin3DTIToolkit PUBLIC ${STATIC_LIBS})
endif()

if (USE_BRT_LIBRARY)
    message("Using BRT library!")
    file(GLOB_RECURSE BRT_LIBRARY 
        "${CMAKE_CURRENT_SOURCE_DIR}/../../BRTLibrary/include/*.hpp" 
        "${CMAKE_CURRENT_SOURCE_DIR}/../../BRTLibrary/include/*.h" 
    )
    source_group(TREE "${CMAKE_CURRENT_SOURCE_DIR}/../../BRTLibrary/include/" PREFIX "BRTLibrary" FILES ${BRT_LIBRARY})
endif()

# Add library
add_library(AudioPlugin3DTIToolkit ${LIB_TYPE}
    ${PROJECT_SRC}
    #${3DTI_TOOLKIT}
    #${3DTI_MODULES}
    #${SOFA}
    #${NETCDF}
    ${BRT_LIBRARY}
)

target_include_directories(AudioPlugin3DTIToolkit PUBLIC 
    ${CMAKE_CURRENT_SOURCE_DIR}/src
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_Toolkit"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/cereal/include"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/eigen"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/src"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../3dti_AudioToolkit/3dti_ResourceManager/third_party_libraries/sofacoustics/libsofa/dependencies/include"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../BRTLibrary/include"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../BRTLibrary/include/third_party_libraries"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../BRTLibrary/include/third_party_libraries/boost_circular_buffer"
)

target_compile_definitions(AudioPlugin3DTIToolkit PRIVATE
    _3DTI_AXIS_CONVENTION_UNITY
    _3DTI_ANGLE_CONVENTION_LISTEN

    # Platform-specific macros
    $<$<PLATFORM_ID:Windows>:DISABLE_SOFA_SUPPORT>
    $<$<PLATFORM_ID:Darwin>:DISABLE_SOFA_SUPPORT>
    $<$<PLATFORM_ID:Linux>:>
    $<$<PLATFORM_ID:Android>:>
    $<$<PLATFORM_ID:iOS>:DISABLE_SOFA_SUPPORT>
)

# macOS .bundle output
message(STATUS "CMAKE_SYSTEM_NAME: ${CMAKE_SYSTEM_NAME}")
if(APPLE AND CMAKE_SYSTEM_NAME STREQUAL "Darwin")
    set_target_properties(AudioPlugin3DTIToolkit PROPERTIES
        BUNDLE TRUE
        MACOSX_BUNDLE TRUE
        MACOSX_BUNDLE_INFO_PLIST "${CMAKE_CURRENT_SOURCE_DIR}/MACOSX/AudioPlugin3DTIToolkit/Info.plist"
        MACOSX_BUNDLE_IDENTIFIER "es.uma.diana.AudioPlugin3DTIWrapper"
        SUFFIX ".bundle"
    )

    target_link_libraries(AudioPlugin3DTIToolkit PRIVATE
        z
        "${CMAKE_CURRENT_SOURCE_DIR}/../../BRTLibrary/include/third_party_libraries/libmysofa/lib/osx/universal/libmysofa.a"
    )

    add_custom_command(TARGET AudioPlugin3DTIToolkit POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy_directory
            "$<TARGET_BUNDLE_DIR:AudioPlugin3DTIToolkit>"
            "${CMAKE_SOURCE_DIR}/../BuildUnityWrapperPackage/Assets/Plugins/macOS/AudioPlugin3DTIToolkit.bundle"
    )
elseif(WIN32)
    target_link_directories(AudioPlugin3DTIToolkit PRIVATE "${CMAKE_SOURCE_DIR}/../../BRTLibrary/include/third_party_libraries/libmysofa/lib/vs/x64")
    target_link_libraries(AudioPlugin3DTIToolkit PRIVATE 
        mysofa
        $<$<CONFIG:Debug>:zlibstaticd>
        $<$<CONFIG:Release>:zlibstatic>
    )

    set(UNITY_PLUGIN_DIR "${CMAKE_SOURCE_DIR}/../BuildUnityWrapperPackage/Assets/Plugins")

    if(CMAKE_SIZEOF_VOID_P EQUAL 8)
        set(UNITY_PLUGIN_DIR "${UNITY_PLUGIN_DIR}/x86_64")
    else()
        set(UNITY_PLUGIN_DIR "${UNITY_PLUGIN_DIR}/x86")
    endif()

    add_custom_command(TARGET AudioPlugin3DTIToolkit POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy
            "$<TARGET_FILE:AudioPlugin3DTIToolkit>"
            "${UNITY_PLUGIN_DIR}/AudioPlugin3DTIToolkit.dll"
    )
elseif(CMAKE_SYSTEM_NAME STREQUAL "Android")
    # Enable specific features or optimizations for Android
    message(STATUS "Android API Level: ${ANDROID_NATIVE_API_LEVEL}")
    message(STATUS "Android ABI: ${ANDROID_ABI}")

    message(STATUS "Libmysofa ${CMAKE_CURRENT_SOURCE_DIR}/lib/libmysofa/android/ARM64/${CMAKE_BUILD_TYPE}")
    target_link_directories(AudioPlugin3DTIToolkit PRIVATE 
        "${CMAKE_CURRENT_SOURCE_DIR}/lib/libmysofa/android/ARM64/${CMAKE_BUILD_TYPE}"
    )
    target_link_libraries(AudioPlugin3DTIToolkit PRIVATE 
        mysofa
        z
    )

    # Define the output path (absolute or relative)
    set(UNITY_PLUGIN_DIR "${CMAKE_SOURCE_DIR}/../BuildUnityWrapperPackage/Assets/Plugins/Android_64")

    # Copy the .so after building
    add_custom_command(TARGET AudioPlugin3DTIToolkit POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E make_directory ${UNITY_PLUGIN_DIR}
        COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:AudioPlugin3DTIToolkit> ${UNITY_PLUGIN_DIR}
        COMMENT "Copying AudioPlugin3DTIToolkit.so to ${UNITY_PLUGIN_DIR}"
    )
endif()
