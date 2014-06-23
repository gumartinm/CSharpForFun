#include <stdlib.h>
#include <iostream>
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/mono-config.h>
 
/**
 * Method to be used from Mono.
 */
static inline void p_Print(MonoString *str) {
    std::cout << "UTF8: " << mono_string_to_utf8(str) << std::endl;
    std::wcout << "UTF16: " << ((wchar_t*)mono_string_to_utf16(str)) << std::endl;
}

/**
 * This program must be run like this: ./test MonoEmbedded.exe
 * (it requires one argument, even when it is not going to be used because I do not know what
 *  parameters to use in mono_jit_exec method, it crashes with NULL value :/)
 */
int main (int argc, char *argv[])
{
    /*
     * Load the default Mono configuration file, this is needed
     * if you are planning on using the dllmaps defined on the
     * system configuration
     */
    mono_config_parse (NULL);

    /*
     * mono_jit_init() creates a domain: each assembly is
     * loaded and run in a MonoDomain.
     */
    MonoDomain *domain = mono_jit_init ("../MonoEmbedded/bin/Debug/MonoEmbedded.exe");

    /*
     * Optionally, add an internal call that your startup.exe
     * code can call, this will bridge startup.exe to Mono
     */
    mono_add_internal_call ("MonoEmbedded.MainClass::Print", (void *)p_Print);

    /*
     * Open the executable, and run the Main method declared
     * in the executable
     */
    MonoAssembly *assembly = mono_domain_assembly_open (domain, "../MonoEmbedded/bin/Debug/MonoEmbedded.exe");
    if (!assembly) {
        exit (2);
    }

    /*
     * mono_jit_exec() will run the Main() method in the assembly.
     * The return value needs to be looked up from
     * System.Environment.ExitCode.
     */
    mono_jit_exec (domain, assembly, argc - 1, argv + 1);

    return 0;
}
