export -configFile adapter_config_delta_test_m_extent.xml -out dump_0_initial.jsonl -force

export -configFile adapter_config_delta_test_m_extent.xml -out dump_1.jsonl -force

export -configFile adapter_config_delta_test_m_extent.xml -out dump_2.jsonl -force


diff -from dump_0_initial.jsonl -to dump_1.jsonl -out delta1.xml -format xml -force

diff -from dump_1.jsonl -to dump_2.jsonl -out delta2.xml -format xml -force
