import sys
import json
import matplotlib.pyplot as plt

def main():
    json_file = sys.argv[1]
    
    with open(json_file, 'r') as file:
        values = json.load(file)
    
    plt.plot(values)
    plt.title(json_file)
    plt.xlabel("time")
    plt.ylabel("ms")
    plt.grid(True)
    plt.show()

if __name__ == "__main__":
    main()